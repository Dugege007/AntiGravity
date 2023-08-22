using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EjectorPool : Singleton<EjectorPool>
{
    //显示当前难度
    [SerializeField] private Level level;

    //简单预制体列表
    public List<EjectorController> easyEjectorProfabList = new List<EjectorController>();
    //困难预制体列表
    public List<EjectorController> hardEjectorProfabList = new List<EjectorController>();
    //挑战预制体列表
    public List<EjectorController> challengeEjectorProfabList = new List<EjectorController>();

    private List<EjectorController> ejectorProfabList;

    //物体出生点
    public List<GameObject> spawnPointList = new List<GameObject>();
    //声明一个数组，用来存放之前生成的X个物体的索引
    private int[] lastSpawnPointIndex = new int[2];
    //出生点索引
    private int spawnIndex;
    //对象池列表
    public List<EjectorController> ejectorList = new List<EjectorController>();
    //声明一个数组，用来存放之前生成的X个物体的索引
    private int[] lastSpawnEjectorIndex = new int[4];
    //随机物体的索引
    private int ejectorIndex;

    protected override void Awake()
    {
        base.Awake();

        //传入level
        level = GameManager.Instance.level;
        switch (level)
        {
            case Level.Easy:
                ejectorProfabList = easyEjectorProfabList;
                break;
            case Level.Hard:
                ejectorProfabList = hardEjectorProfabList;
                break;
            case Level.Challenge:
                ejectorProfabList = challengeEjectorProfabList;
                break;
            case Level.Test:
                ejectorProfabList = easyEjectorProfabList;
                break;
        }
    }

    private void OnEnable()
    {
        //初始出生点位置
        spawnIndex = 1;

        EventHandler.LaunchEjectorEvent += OnLaunchEjectorEvent;
    }

    private void OnDisable()
    {
        EventHandler.LaunchEjectorEvent -= OnLaunchEjectorEvent;
    }

    private void OnLaunchEjectorEvent()
    {
        //检查之前的喷射物
        lastSpawnEjectorIndex[0] = lastSpawnEjectorIndex[1];
        lastSpawnEjectorIndex[1] = lastSpawnEjectorIndex[2];
        lastSpawnEjectorIndex[2] = lastSpawnEjectorIndex[3];

        //检查之前的出生点
        lastSpawnEjectorIndex[0] = lastSpawnEjectorIndex[1];

        //随机喷射物
        //如果前X次已经包含这次的索引，则再次随机
        while (lastSpawnEjectorIndex.Contains(ejectorIndex))
            ejectorIndex = Random.Range(0, ejectorProfabList.Count);
        lastSpawnEjectorIndex[3] = ejectorIndex;

        //随机出生点
        //如果前X次已经包含这次的索引，则再次随机
        while (lastSpawnPointIndex.Contains(spawnIndex))
            spawnIndex = Random.Range(0, spawnPointList.Count);
        lastSpawnPointIndex[1] = spawnIndex;

        //遍历对象池
        foreach (var ejector in ejectorList)
        {
            //如果对象池内已经存在该物体，并且已准备好
            if (ejector.name == ejectorProfabList[ejectorIndex].name + "(Clone)" && !ejector.gameObject.activeInHierarchy)
            {
                //则将其在出生点启用，然后直接返回
                ejector.gameObject.SetActive(true);
                ejector.gameObject.transform.position = spawnPointList[spawnIndex].transform.position;
                return;
            }
        }

        SpawnEjector();
    }

    private void SpawnEjector()
    {
        //生成新的物体
        var ejectorPrefab = Instantiate(ejectorProfabList[ejectorIndex], spawnPointList[spawnIndex].transform.position, Quaternion.identity);
        ejectorPrefab.transform.parent = transform;
        ejectorList.Add(ejectorPrefab);
    }
}
