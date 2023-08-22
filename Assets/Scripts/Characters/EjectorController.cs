using System.Net;
using UnityEngine;

public class EjectorController : MonoBehaviour
{
    //显示当前难度
    [SerializeField] private Level level;

    public GameObject particlePool;
    //粒子预制体
    public ParticleSystem particlePrefab;
    //绕旋的目标
    private GameObject rotateAroundTarget;
    //喷射物数据
    protected EjectorStats ejectroStats;
    //玩家数据
    protected PlayerStats playerStats;
    //母星数据
    protected ParentStarStats parentStarStats;
    //刚体组件
    private Rigidbody2D rb;
    //原来的比例
    private Vector3 baseScale;
    //绕旋角速度
    private float angleSpeed;

    //开始回血的时间
    private float restoreHPStart;
    //回血时间计时器
    public float restoreHPTime;
    //开始无敌时间
    private float timeInvincible;
    //无敌时间计时器
    public float invincibleTimer;

    //开始等待时间
    private float startTime;
    //返回对象池时间
    public float returnTime;
    //是否已经消失
    protected bool isDisappeared;

    private void Awake()
    {
        playerStats = PlayerController.Instance.playerStats;
        parentStarStats = ParentStarController.Instance.parentStarStats;
        ejectroStats = GetComponent<EjectorStats>();
        rb = GetComponent<Rigidbody2D>();
        baseScale = transform.localScale;
    }

    protected virtual void OnEnable()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        isDisappeared = false;
        rotateAroundTarget = GameObject.FindGameObjectWithTag("ParentStar");

        restoreHPStart = Time.time;
        timeInvincible = Time.time;
    }

    private void Update()
    {
        //每隔一定时间，增加血量
        if (ejectroStats.CurrentHealth != ejectroStats.MaxHealth && Time.time >= restoreHPStart + restoreHPTime)
        {
            restoreHPStart = Time.time;
            ejectroStats.CurrentHealth++;
        }

        //距离太远掉血
        if (Vector3.Distance(rb.transform.position, parentStarStats.transform.position) > parentStarStats.ControlDistance && Time.time > timeInvincible + invincibleTimer)
        {
            timeInvincible = Time.time;
            ejectroStats.CurrentHealth--;
        }

        switch (level)
        {
            case Level.Easy:
                //玩家距离障碍物太近，发出提示
                if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < 1.5 && CompareTag("Obstacle"))
                    EventHandler.CallGetHitByEjectorEvent();
                //提示玩家收集Buff
                if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < 1.5 && (CompareTag("Buff") || CompareTag("Prop")))
                    EventHandler.CallGetBuffByEjectorEvent();
                break;
            case Level.Hard:
            case Level.Challenge:
                break;
        }
    }

    private void FixedUpdate()
    {

        if (rb.bodyType == RigidbodyType2D.Dynamic && !isDisappeared)
        {
            //自转
            rb.transform.Rotate(Vector3.forward, ejectroStats.BaseSelfAngleSpeed * Time.deltaTime);

            //切换绕飞目标，并收到其重力影响
            SwithAroundTarget();

            //绕转弧长速度
            angleSpeed = ejectroStats.BaseArcLengthSpeed * 180 / (Mathf.PI * Vector2.Distance(rotateAroundTarget.transform.position, rb.transform.position));
            rb.transform.RotateAround(rotateAroundTarget.transform.position, Vector3.forward, angleSpeed * Time.deltaTime);//HACK transform.RotateAround()过时了吗？可以用什么代替？

            //限制速度大小
            if (rb.velocity.magnitude >= ejectroStats.MaxSpeed)
                rb.velocity = rb.velocity.normalized * ejectroStats.MaxSpeed;
        }

        //如果 HP == 0，喷射物消失并回到母星位置，开始等待
        if (ejectroStats.CurrentHealth == 0 && !isDisappeared)
            ReturnToParentStar();

        //如果到returnTime，关闭对象
        if (Time.time > startTime + returnTime && isDisappeared)
            gameObject.SetActive(false);
    }

    /// <summary>
    /// 切换绕旋物体
    /// </summary>
    private void SwithAroundTarget()
    {
        if (Vector3.Distance(PlayerController.Instance.transform.position, transform.position) <= playerStats.SkillDistance && CompareTag("Buff"))
        {
            //更换绕旋目标
            rotateAroundTarget = GameObject.FindGameObjectWithTag("Player");
            //比例变小
            transform.localScale = baseScale * (Vector3.Distance(PlayerController.Instance.transform.position, transform.position) / playerStats.SkillDistance) * 0.5f + baseScale * 0.5f;
            //重力作用
            GravityEffect(playerStats.Gravitation);
        }
        else if (Vector3.Distance(PlayerController.Instance.transform.position, transform.position) <= playerStats.SkillDistance * 0.6f && CompareTag("Prop"))
        {
            //更换绕旋目标
            rotateAroundTarget = GameObject.FindGameObjectWithTag("Player");
            //比例变小
            transform.localScale = baseScale * (Vector3.Distance(PlayerController.Instance.transform.position, transform.position) / playerStats.SkillDistance) * 0.5f + baseScale * 0.8f;
            //重力作用
            GravityEffect(playerStats.Gravitation);
        }
        else
        {
            transform.localScale = baseScale;
            rotateAroundTarget = GameObject.FindGameObjectWithTag("ParentStar");
            //重力作用
            GravityEffect(parentStarStats.CurrentGravitation);
        }
    }

    /// <summary>
    /// 受到引力影响
    /// </summary>
    /// <param name="othersGravity">目标的引力值</param>
    private void GravityEffect(float othersGravity)
    {
        //受到重力源的影响
        Vector2 gravityDirection = (Vector2)rotateAroundTarget.transform.position - rb.position;
        //万有引力公式，计算当前受到的重力
        Vector2 gravity = gravityDirection.normalized * (othersGravity / Mathf.Pow(gravityDirection.magnitude, 1.5f));

        if (gravity.magnitude > 0.0001f)    //避免引力过小导致空值错误
            rb.AddForce(gravity);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        UpdateAfterCollision(collision.gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        UpdateAfterCollision(collision.gameObject);
    }

    private void UpdateAfterCollision(GameObject obj)
    {
        if (obj.gameObject.CompareTag("Player") && !isDisappeared)
        {
            ParticlePool.Instance.GetFromPool(this.gameObject);
            //玩家获得数据
            playerStats.GetBuff(ejectroStats.buffData);
            //更新得分
            UIManager.Instance.UpdateScore();
            //更新血量
            UIManager.Instance.UpdateHealth();
            ejectroStats.CurrentHealth--;
        }
    }

    public void ReturnToParentStar()
    {
        startTime = Time.time;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.transform.position += Vector3.forward;
        rb.transform.position = ParentStarController.Instance.transform.position + Vector3.forward;
        isDisappeared = true;
    }
}
