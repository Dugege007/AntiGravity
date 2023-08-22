using System.Collections.Generic;
using UnityEngine;

public class ShadowPool : Singleton<ShadowPool>
{
    public GameObject shadowPrefab;
    public int shadowCount;

    //对象池队列
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

            //取消启用，返回对象池
            ReturnPool(newShadow);
        }
    }

    public void ReturnPool(GameObject gameObject)
    {
        gameObject.SetActive(false);

        //添加到队列末端
        availableObjects.Enqueue(gameObject);
    }

    public GameObject GetFromPool()
    {
        if (availableObjects.Count == 0)
        {
            FillPool();
        }
        var outShadow = availableObjects.Dequeue();
        outShadow.SetActive(true);  // 当 SetActive(true) 时，会执行一次OnEnable()
        return outShadow;
    }
}
