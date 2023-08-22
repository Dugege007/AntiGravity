using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static Cinemachine.DocumentationSortingAttribute;

public class PlayerController : Singleton<PlayerController>
{
    //��ʾ��ǰ�Ѷ�
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
    //��Ӱ���ʱ��
    public float shadowDeltaTime;
    //��Ӱ��ʱ��
    private float shadowTimer;
    //ֹͣ����
    public float stopDistance;
    //�޵�ʱ��
    public float timeInvincible;
    //�޵�ʱ���ʱ��
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

        //�����µ�InputSystemʵ��
        inputControls = new InputControls();

        //��ѡһ�²ٿط�ʽ
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
        //�жϵ�ǰѪ���Ƿ�Ϊ0�����Ϊ0�����isDead��ֵtrue
        isDead = playerStats.CurrentHealth <= 0;

        //�泯ĸ��
        LookAtTarget();

        //����ָ����ɫ
        UpdatePointerColor();

        //����̫Զ��ʾ
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
        //����̫Զ��Ѫ������˸
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

        //�����ٶȴ�С
        if (rb.velocity.magnitude >= playerStats.MaxSpeed)
        {
            rb.velocity = rb.velocity.normalized * playerStats.MaxSpeed;
        }
    }

    private void LookAtTarget()
    {
        //��ȡ��ҵ�ĸ��λ�õ�����
        Vector3 PlayerToParentStar = ParentStarController.Instance.transform.position - transform.position;
        //����Ƕ�
        float angle = Vector3.SignedAngle(Vector3.up, PlayerToParentStar, Vector3.forward);
        //�������������ĸ��
        rb.transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, 0, angle), 5);
    }

    private void GravityEffect()
    {
        //����ܵ�����Դ��Ӱ��
        Vector2 gravityDirection = (Vector2)ParentStarController.Instance.transform.position - rb.position;
        //����������ʽ��������ҵ�ǰ�ܵ�������
        Vector2 gravity = gravityDirection.normalized * ParentStarController.Instance.parentStarStats.CurrentGravitation / Mathf.Pow(gravityDirection.magnitude, 2);

        rb.AddForce(gravity);
    }

    private void PlayerMove()
    {
        if (isPressed && canMove)
        {
            // ��Ӱ��Ч
            shadowTimer += Time.deltaTime;
            if (shadowTimer > shadowDeltaTime)
            {
                ShadowPool.Instance.GetFromPool();
                shadowDeltaTime = 0;
            }

            if (canPlayFX)
            {
                //�����ƶ���Ч
                PlayMoveMusic();
                //�ر�β��
                tailGas.Stop();
                canPlayFX = false;
            }

            //if (playerInput.currentControlScheme == "Keyboard" || playerInput.currentControlScheme == "GamePad")
            switch (GameManager.Instance.controlMode)
            {
                case OperationMode.GamePad:
                case OperationMode.KeyBoard:
                    //���̺�ҡ�˿����ƶ��ٶ�
                    rb.velocity = moveDirection * playerStats.MaxSpeed;
                    break;
                case OperationMode.MouseClick:
                case OperationMode.Touch:
                    //��Ļ����ת��Ϊ���������
                    pressPosition = Camera.main.ScreenToWorldPoint(inputControls.PlayerMT.Direction.ReadValue<Vector2>());
                    //�����Ļ��������ƶ��ٶ�
                    rb.velocity = (pressPosition - rb.position).normalized * playerStats.MaxSpeed;
                    break;
            }

            //�ƶ���ʱ
            currentMoveTime -= Time.deltaTime;
            if (currentMoveTime < 0)
            {
                //������ȴ
                moveCoolingTime = 0;

                canMove = false;
                tailGas.Play();
            }

            //������ȴ
            moveCoolingTime = 0;
        }
    }

    private void MoveStarted(InputAction.CallbackContext context)
    {
        isPressed = true;
    }

    private void MovePerformed(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>().normalized;    //HACK ֱ���Ƶ������
    }

    private void MoveCanceled(InputAction.CallbackContext context)
    {
        moveDirection = Vector2.zero;
        isPressed = false;
        tailGas.Play();
    }

    /// <summary>
    /// �����ƶ�ʱ��
    /// </summary>
    private void ResetMoveTime()
    {
        if (!isPressed || !canMove)
        {
            canMove = false;

            //��������
            canPlayFX = true;

            //�ƶ���ȴʱ��˥��
            moveCoolingTime += Time.deltaTime;
            if (moveCoolingTime >= playerStats.MoveCoolDownTime)
            {
                canMove = true;
                //�����ƶ�����ʱ��
                currentMoveTime = playerStats.MaxMoveTime;
            }
        }
    }

    //����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeadPoint") && isDead)
        {
            //TODO �����������ֵ+2���ر�GameOver����ʾ����ʾ����ָ�룬�������λ��
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
    /// ��ʾ���Ʒ�ΧGizmos
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, skillDistance);
    }
}
