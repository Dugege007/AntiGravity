using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static Cinemachine.DocumentationSortingAttribute;

public class PlayerController : Singleton<PlayerController>
{
    //显示当前难度
    [SerializeField] private Level level;

    public Rigidbody2D rb;
    public GameObject directionPointer;
    public Transform deadPoint;
    public ParticleSystem collisionParticle;
    public ParticleSystem tailGas;
    public InputControls inputControls;
    private PlayerInput playerInput;

    [Header("Move")]
    private Vector2 pressPosition;
    private Vector2 moveDirection;
    private bool canPlayFX;
    public bool canMove;
    //残影间隔时间
    public float shadowDeltaTime;
    //残影计时器
    private float shadowTimer;
    //停止距离
    public float stopDistance;
    //无敌时间
    public float timeInvincible;
    //无敌时间计时器
    private float invincibleTimer;
    private SpriteRenderer playerColor;

    public float skillDistance;

    [HideInInspector]
    public PlayerStats playerStats;
    private ParentStarStats parentStarStats;
    [HideInInspector]
    public float currentMoveTime;
    [HideInInspector]
    public float moveCoolingTime;
    [HideInInspector]
    public bool isPressed;

    //RGB
    private float r;
    private float g;
    //private float b;
    private float a;

    public bool isDead;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody2D>();
        playerStats = GetComponent<PlayerStats>();
        playerColor = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>();
        parentStarStats = GameObject.FindGameObjectWithTag("ParentStar").GetComponent<ParentStarStats>();

        //引入新的InputSystem实例
        inputControls = new InputControls();

        //先选一下操控方式
        switch (GameManager.Instance.controlMode)
        {
            case OperationMode.GamePad:
            case OperationMode.KeyBoard:
                inputControls.PlayerKG.Enable();
                inputControls.PlayerMT.Disable();
                break;
            case OperationMode.MouseClick:
            case OperationMode.Touch:
                inputControls.PlayerKG.Disable();
                inputControls.PlayerMT.Enable();
                break;
        }

        level = GameManager.Instance.level;

        isDead = false;
        canMove = true;
        currentMoveTime = playerStats.MaxMoveTime;
    }

    private void OnEnable()
    {
        inputControls.PlayerKG.Move.started += MoveStarted;
        inputControls.PlayerKG.Move.performed += MovePerformed;
        inputControls.PlayerKG.Move.canceled += MoveCanceled;

        inputControls.PlayerMT.Move.started += MoveStarted;
        inputControls.PlayerMT.Direction.performed += MovePerformed;
        inputControls.PlayerMT.Move.canceled += MoveCanceled;
    }

    private void OnDisable()
    {
        inputControls.PlayerKG.Move.started -= MoveStarted;
        inputControls.PlayerKG.Move.performed -= MovePerformed;
        inputControls.PlayerKG.Move.canceled -= MoveCanceled;

        inputControls.PlayerMT.Move.started -= MoveStarted;
        inputControls.PlayerMT.Direction.performed -= MovePerformed;
        inputControls.PlayerMT.Move.canceled -= MoveCanceled;
    }

    private void Update()
    {
        //判断当前血量是否为0，如果为0，则给isDead赋值true
        isDead = playerStats.CurrentHealth <= 0;

        //面朝母星
        LookAtTarget();

        //更新指针颜色
        UpdatePointerColor();

        //距离太远提示
        switch (level)
        {
            case Level.Easy:
                if (Vector3.Distance(rb.transform.position, deadPoint.transform.position) > parentStarStats.ControlDistance - 1.5)
                    EventHandler.CallTooFarToParentEvent();
                break;
            case Level.Hard:
            case Level.Challenge:
                break;
        }

        playerColor.color = new Color(1, 1, 1, 1);
        //距离太远掉血，并闪烁
        if (Vector3.Distance(rb.transform.position, deadPoint.transform.position) > parentStarStats.ControlDistance)
        {
            a = Mathf.Max(invincibleTimer / timeInvincible, 0);
            playerColor.color = new Color(a, a, a, a);

            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
            {
                playerStats.GetHurt();
                UIManager.Instance.UpdateHealth();

                invincibleTimer = timeInvincible;
            }
        }

        if (isDead)
        {
            rb.velocity = Vector3.zero;

            StartCoroutine(BackToParentStar());
        }
        else
        {
            ResetMoveTime();
        }
    }

    private void FixedUpdate()
    {
        if (!ParentStarController.Instance.isArrived && !isDead)
        {
            GravityEffect();
            PlayerMove();
        }

        //限制速度大小
        if (rb.velocity.magnitude >= playerStats.MaxSpeed)
        {
            rb.velocity = rb.velocity.normalized * playerStats.MaxSpeed;
        }
    }

    private void LookAtTarget()
    {
        //获取玩家到母星位置的向量
        Vector3 PlayerToParentStar = ParentStarController.Instance.transform.position - transform.position;
        //计算角度
        float angle = Vector3.SignedAngle(Vector3.up, PlayerToParentStar, Vector3.forward);
        //玩家自旋，朝向母星
        rb.transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, 0, angle), 5);
    }

    private void GravityEffect()
    {
        //玩家受到重力源的影响
        Vector2 gravityDirection = (Vector2)ParentStarController.Instance.transform.position - rb.position;
        //万有引力公式，计算玩家当前受到的重力
        Vector2 gravity = gravityDirection.normalized * ParentStarController.Instance.parentStarStats.CurrentGravitation / Mathf.Pow(gravityDirection.magnitude, 2);

        rb.AddForce(gravity);
    }

    private void PlayerMove()
    {
        if (isPressed && canMove)
        {
            // 残影特效
            shadowTimer += Time.deltaTime;
            if (shadowTimer > shadowDeltaTime)
            {
                ShadowPool.Instance.GetFromPool();
                shadowDeltaTime = 0;
            }

            if (canPlayFX)
            {
                //播放移动音效
                PlayMoveMusic();
                //关闭尾气
                tailGas.Stop();
                canPlayFX = false;
            }

            //if (playerInput.currentControlScheme == "Keyboard" || playerInput.currentControlScheme == "GamePad")
            switch (GameManager.Instance.controlMode)
            {
                case OperationMode.GamePad:
                case OperationMode.KeyBoard:
                    //键盘和摇杆控制移动速度
                    rb.velocity = moveDirection * playerStats.MaxSpeed;
                    break;
                case OperationMode.MouseClick:
                case OperationMode.Touch:
                    //屏幕坐标转换为世界坐标点
                    pressPosition = Camera.main.ScreenToWorldPoint(inputControls.PlayerMT.Direction.ReadValue<Vector2>());
                    //点击屏幕控制玩家移动速度
                    rb.velocity = (pressPosition - rb.position).normalized * playerStats.MaxSpeed;
                    break;
            }

            //移动计时
            currentMoveTime -= Time.deltaTime;
            if (currentMoveTime < 0)
            {
                //重置冷却
                moveCoolingTime = 0;

                canMove = false;
                tailGas.Play();
            }

            //重置冷却
            moveCoolingTime = 0;
        }
    }

    private void MoveStarted(InputAction.CallbackContext context)
    {
        isPressed = true;
    }

    private void MovePerformed(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>().normalized;    //HACK 直接推到最大速
    }

    private void MoveCanceled(InputAction.CallbackContext context)
    {
        moveDirection = Vector2.zero;
        isPressed = false;
        tailGas.Play();
    }

    /// <summary>
    /// 重置移动时间
    /// </summary>
    private void ResetMoveTime()
    {
        if (!isPressed || !canMove)
        {
            canMove = false;

            //重置声音
            canPlayFX = true;

            //移动冷却时间衰减
            moveCoolingTime += Time.deltaTime;
            if (moveCoolingTime >= playerStats.MoveCoolDownTime)
            {
                canMove = true;
                //重置移动持续时间
                currentMoveTime = playerStats.MaxMoveTime;
            }
        }
    }

    //死亡
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeadPoint") && isDead)
        {
            //TODO 看完广告后生命值+2，关闭GameOver和提示，显示方向指针，重置玩家位置
            UIManager.Instance.GamePause();
            UIManager.Instance.resumeBtn.gameObject.SetActive(false);
            UIManager.Instance.gameOver.gameObject.SetActive(true);

            EventHandler.CallGetPointEvent(playerStats.FinalPoint);
            EventHandler.CallEndGameEvent();

            gameObject.SetActive(false);
            rb.transform.position = deadPoint.transform.position;
        }
    }

    private void UpdatePointerColor()
    {
        r = Mathf.Min(Vector3.Distance(rb.transform.position, ParentStarController.Instance.transform.position) / parentStarStats.ControlDistance, 1);
        g = Mathf.Min(1 - Vector3.Distance(rb.transform.position, ParentStarController.Instance.transform.position) / parentStarStats.ControlDistance, 1);
        directionPointer.GetComponent<SpriteRenderer>().color = new Color(r, g, 0);
    }

    private void PlayMoveMusic()
    {
        AudioManager.Instance.PlayMoveFX();
    }

    public IEnumerator BackToParentStar()
    {
        float speed = 2;
        rb.transform.position = Vector3.Lerp(rb.transform.position, ParentStarController.Instance.transform.position, Time.deltaTime * speed);
        yield return null;
    }

    /// <summary>
    /// 显示控制范围Gizmos
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, skillDistance);
    }
}
