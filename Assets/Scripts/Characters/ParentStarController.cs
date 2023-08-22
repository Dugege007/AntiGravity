using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParentStarController : Singleton<ParentStarController>
{
    //显示当前难度
    [SerializeField] private Level level;
    //开始生成时间
    private float startSpawnTime;
    //生成物体的冷却时间
    private float spawnCoolDownTime;
    //随机特殊物体出现时间
    //private float specialTime;
    //是否到达母星
    public bool isArrived;
    //到达动画计时
    private float animIimer;
    //等待时间
    public float waitTime;

    [HideInInspector]
    //数据
    public ParentStarStats parentStarStats;

    protected override void Awake()
    {
        base.Awake();

        //传入level
        level = GameManager.Instance.level;
        switch (level)
        {
            case Level.Easy:
                waitTime = 5;
                break;
            case Level.Hard:
                waitTime = 4;
                break;
            case Level.Challenge:
                waitTime = 3;
                break;
            case Level.Test:
                waitTime = 1;
                break;
        }

        parentStarStats = GetComponent<ParentStarStats>();
        spawnCoolDownTime = waitTime;
        startSpawnTime = Time.time;
        animIimer = 0;
    }

    private void Update()
    {
        if (!PlayerController.Instance.isDead)
        {
            LaunchEjector();

            if (isArrived)
                PlayerArrived();
        }
    }

    private void FixedUpdate()
    {
        //自旋
        transform.Rotate(Vector3.forward, parentStarStats.AngleSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 生成E喷射物，并添加到对象池
    /// </summary>
    private void LaunchEjector()
    {
        if (Time.time > startSpawnTime + spawnCoolDownTime && !isArrived)
        {
            EventHandler.CallLaunchEjectorEvent();
            //重置生成物体时间
            spawnCoolDownTime = Random.Range(parentStarStats.MaxSpawnTime * 0.5f, parentStarStats.MaxSpawnTime);
            startSpawnTime = Time.time;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //关掉所有喷射物
            foreach (var ejector in EjectorPool.Instance.ejectorList)
            {
                ejector.ReturnToParentStar();
            }

            if (!PlayerController.Instance.isDead && PlayerController.Instance.playerStats.FinalPoint != 0)
            {
                EventHandler.CallArrivedParentEvent();
                isArrived = true;
            }

            PlayerController.Instance.rb.velocity = Vector3.zero;
            PlayerController.Instance.rb.bodyType = RigidbodyType2D.Kinematic;
            PlayerController.Instance.canMove = false;
        }

        if (collision.CompareTag("Player") && PlayerController.Instance.playerStats.FinalPoint != 0)
        {
            //升级
            parentStarStats.parentStarData.LevelUp();
        }
    }

    //玩家到达动画
    private void PlayerArrived()
    {
        float speed = 6;
        animIimer += Time.deltaTime;

        if (animIimer < waitTime)
        {
            PlayerController.Instance.rb.transform.position = Vector3.Lerp(PlayerController.Instance.rb.transform.position, transform.position, Time.deltaTime * speed);
        }
        else
        {
            speed = 3;

            PlayerController.Instance.rb.transform.position = Vector3.Lerp(PlayerController.Instance.rb.transform.position, new Vector3(0, -3, 0), Time.deltaTime * speed);

            if (Vector3.Distance(PlayerController.Instance.rb.transform.position, transform.position) > 2)
            {
                PlayerController.Instance.rb.bodyType = RigidbodyType2D.Dynamic;
                PlayerController.Instance.canMove = true;
                isArrived = false;

                EventHandler.CallLeaveParentEvent();
                animIimer = 0;
            }
        }
    }

    /// <summary>
    /// 显示控制范围Gizmos
    /// </summary>
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, parentStarStats.ControlDistance);
    //}
}
