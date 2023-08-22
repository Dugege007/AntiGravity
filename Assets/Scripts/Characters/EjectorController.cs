using System.Net;
using UnityEngine;

public class EjectorController : MonoBehaviour
{
    //��ʾ��ǰ�Ѷ�
    [SerializeField] private Level level;

    public GameObject particlePool;
    //����Ԥ����
    public ParticleSystem particlePrefab;
    //������Ŀ��
    private GameObject rotateAroundTarget;
    //����������
    protected EjectorStats ejectroStats;
    //�������
    protected PlayerStats playerStats;
    //ĸ������
    protected ParentStarStats parentStarStats;
    //�������
    private Rigidbody2D rb;
    //ԭ���ı���
    private Vector3 baseScale;
    //�������ٶ�
    private float angleSpeed;

    //��ʼ��Ѫ��ʱ��
    private float restoreHPStart;
    //��Ѫʱ���ʱ��
    public float restoreHPTime;
    //��ʼ�޵�ʱ��
    private float timeInvincible;
    //�޵�ʱ���ʱ��
    public float invincibleTimer;

    //��ʼ�ȴ�ʱ��
    private float startTime;
    //���ض����ʱ��
    public float returnTime;
    //�Ƿ��Ѿ���ʧ
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
        //ÿ��һ��ʱ�䣬����Ѫ��
        if (ejectroStats.CurrentHealth != ejectroStats.MaxHealth && Time.time >= restoreHPStart + restoreHPTime)
        {
            restoreHPStart = Time.time;
            ejectroStats.CurrentHealth++;
        }

        //����̫Զ��Ѫ
        if (Vector3.Distance(rb.transform.position, parentStarStats.transform.position) > parentStarStats.ControlDistance && Time.time > timeInvincible + invincibleTimer)
        {
            timeInvincible = Time.time;
            ejectroStats.CurrentHealth--;
        }

        switch (level)
        {
            case Level.Easy:
                //��Ҿ����ϰ���̫����������ʾ
                if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < 1.5 && CompareTag("Obstacle"))
                    EventHandler.CallGetHitByEjectorEvent();
                //��ʾ����ռ�Buff
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
            //��ת
            rb.transform.Rotate(Vector3.forward, ejectroStats.BaseSelfAngleSpeed * Time.deltaTime);

            //�л��Ʒ�Ŀ�꣬���յ�������Ӱ��
            SwithAroundTarget();

            //��ת�����ٶ�
            angleSpeed = ejectroStats.BaseArcLengthSpeed * 180 / (Mathf.PI * Vector2.Distance(rotateAroundTarget.transform.position, rb.transform.position));
            rb.transform.RotateAround(rotateAroundTarget.transform.position, Vector3.forward, angleSpeed * Time.deltaTime);//HACK transform.RotateAround()��ʱ���𣿿�����ʲô���棿

            //�����ٶȴ�С
            if (rb.velocity.magnitude >= ejectroStats.MaxSpeed)
                rb.velocity = rb.velocity.normalized * ejectroStats.MaxSpeed;
        }

        //��� HP == 0����������ʧ���ص�ĸ��λ�ã���ʼ�ȴ�
        if (ejectroStats.CurrentHealth == 0 && !isDisappeared)
            ReturnToParentStar();

        //�����returnTime���رն���
        if (Time.time > startTime + returnTime && isDisappeared)
            gameObject.SetActive(false);
    }

    /// <summary>
    /// �л���������
    /// </summary>
    private void SwithAroundTarget()
    {
        if (Vector3.Distance(PlayerController.Instance.transform.position, transform.position) <= playerStats.SkillDistance && CompareTag("Buff"))
        {
            //��������Ŀ��
            rotateAroundTarget = GameObject.FindGameObjectWithTag("Player");
            //������С
            transform.localScale = baseScale * (Vector3.Distance(PlayerController.Instance.transform.position, transform.position) / playerStats.SkillDistance) * 0.5f + baseScale * 0.5f;
            //��������
            GravityEffect(playerStats.Gravitation);
        }
        else if (Vector3.Distance(PlayerController.Instance.transform.position, transform.position) <= playerStats.SkillDistance * 0.6f && CompareTag("Prop"))
        {
            //��������Ŀ��
            rotateAroundTarget = GameObject.FindGameObjectWithTag("Player");
            //������С
            transform.localScale = baseScale * (Vector3.Distance(PlayerController.Instance.transform.position, transform.position) / playerStats.SkillDistance) * 0.5f + baseScale * 0.8f;
            //��������
            GravityEffect(playerStats.Gravitation);
        }
        else
        {
            transform.localScale = baseScale;
            rotateAroundTarget = GameObject.FindGameObjectWithTag("ParentStar");
            //��������
            GravityEffect(parentStarStats.CurrentGravitation);
        }
    }

    /// <summary>
    /// �ܵ�����Ӱ��
    /// </summary>
    /// <param name="othersGravity">Ŀ�������ֵ</param>
    private void GravityEffect(float othersGravity)
    {
        //�ܵ�����Դ��Ӱ��
        Vector2 gravityDirection = (Vector2)rotateAroundTarget.transform.position - rb.position;
        //����������ʽ�����㵱ǰ�ܵ�������
        Vector2 gravity = gravityDirection.normalized * (othersGravity / Mathf.Pow(gravityDirection.magnitude, 1.5f));

        if (gravity.magnitude > 0.0001f)    //����������С���¿�ֵ����
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
            //��һ������
            playerStats.GetBuff(ejectroStats.buffData);
            //���µ÷�
            UIManager.Instance.UpdateScore();
            //����Ѫ��
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
