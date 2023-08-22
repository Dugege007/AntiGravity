using System.Collections.Generic;
using UnityEngine;

public class ShadowPool : Singleton<ShadowPool>
{
    public GameObject shadowPrefab;
    public int shadowCount;

    //����ض���
    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    protected override void Awake()
    {
        base.Awake();
    }

    public void FillPool()
    {
        for (int i = 0; i < shadowCount; i++)
        {
            var newShadow = Instantiate(shadowPrefab);
            newShadow.transform.SetParent(transform);

            //ȡ�����ã����ض����
            ReturnPool(newShadow);
        }
    }

    public void ReturnPool(GameObject gameObject)
    {
        gameObject.SetActive(false);

        //��ӵ�����ĩ��
        availableObjects.Enqueue(gameObject);
    }

    public GameObject GetFromPool()
    {
        if (availableObjects.Count == 0)
        {
            FillPool();
        }
        var outShadow = availableObjects.Dequeue();
        outShadow.SetActive(true);  // �� SetActive(true) ʱ����ִ��һ��OnEnable()
        return outShadow;
    }
}
