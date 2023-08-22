using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EjectorPool : Singleton<EjectorPool>
{
    //��ʾ��ǰ�Ѷ�
    [SerializeField] private Level level;

    //��Ԥ�����б�
    public List<EjectorController> easyEjectorProfabList = new List<EjectorController>();
    //����Ԥ�����б�
    public List<EjectorController> hardEjectorProfabList = new List<EjectorController>();
    //��սԤ�����б�
    public List<EjectorController> challengeEjectorProfabList = new List<EjectorController>();

    private List<EjectorController> ejectorProfabList;

    //���������
    public List<GameObject> spawnPointList = new List<GameObject>();
    //����һ�����飬�������֮ǰ���ɵ�X�����������
    private int[] lastSpawnPointIndex = new int[2];
    //����������
    private int spawnIndex;
    //������б�
    public List<EjectorController> ejectorList = new List<EjectorController>();
    //����һ�����飬�������֮ǰ���ɵ�X�����������
    private int[] lastSpawnEjectorIndex = new int[4];
    //������������
    private int ejectorIndex;

    protected override void Awake()
    {
        base.Awake();

        //����level
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
        //��ʼ������λ��
        spawnIndex = 1;

        EventHandler.LaunchEjectorEvent += OnLaunchEjectorEvent;
    }

    private void OnDisable()
    {
        EventHandler.LaunchEjectorEvent -= OnLaunchEjectorEvent;
    }

    private void OnLaunchEjectorEvent()
    {
        //���֮ǰ��������
        lastSpawnEjectorIndex[0] = lastSpawnEjectorIndex[1];
        lastSpawnEjectorIndex[1] = lastSpawnEjectorIndex[2];
        lastSpawnEjectorIndex[2] = lastSpawnEjectorIndex[3];

        //���֮ǰ�ĳ�����
        lastSpawnEjectorIndex[0] = lastSpawnEjectorIndex[1];

        //���������
        //���ǰX���Ѿ�������ε����������ٴ����
        while (lastSpawnEjectorIndex.Contains(ejectorIndex))
            ejectorIndex = Random.Range(0, ejectorProfabList.Count);
        lastSpawnEjectorIndex[3] = ejectorIndex;

        //���������
        //���ǰX���Ѿ�������ε����������ٴ����
        while (lastSpawnPointIndex.Contains(spawnIndex))
            spawnIndex = Random.Range(0, spawnPointList.Count);
        lastSpawnPointIndex[1] = spawnIndex;

        //���������
        foreach (var ejector in ejectorList)
        {
            //�����������Ѿ����ڸ����壬������׼����
            if (ejector.name == ejectorProfabList[ejectorIndex].name + "(Clone)" && !ejector.gameObject.activeInHierarchy)
            {
                //�����ڳ��������ã�Ȼ��ֱ�ӷ���
                ejector.gameObject.SetActive(true);
                ejector.gameObject.transform.position = spawnPointList[spawnIndex].transform.position;
                return;
            }
        }

        SpawnEjector();
    }

    private void SpawnEjector()
    {
        //�����µ�����
        var ejectorPrefab = Instantiate(ejectorProfabList[ejectorIndex], spawnPointList[spawnIndex].transform.position, Quaternion.identity);
        ejectorPrefab.transform.parent = transform;
        ejectorList.Add(ejectorPrefab);
    }
}
