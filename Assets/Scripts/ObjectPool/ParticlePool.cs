using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : Singleton<ParticlePool>
{
    public GameObject particlePrefab;
    public int particleCount;

    //对象池队列
    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    protected override void Awake()
    {
        base.Awake();
    }

    public void FillPool()
    {
        for (int i = 0; i < particleCount; i++)
        {
            GameObject newParticle = Instantiate(particlePrefab);
            newParticle.transform.SetParent(transform);

            //取消启用，返回对象池
            ReturnPool(newParticle);
        }
    }

    public void ReturnPool(GameObject obj)
    {
        //obj.gameObject.SetActive(false);

        //添加到队列末端
        availableObjects.Enqueue(obj);
    }

    public GameObject GetFromPool(GameObject obj)
    {
        if (availableObjects.Count == 0)
        {
            FillPool();
        }
        GameObject outParticle = availableObjects.Dequeue();
        outParticle.gameObject.SetActive(true);  // 当 SetActive(true) 时，会执行一次OnEnable()
        //transform.position = player.position + (obj.transform.position - player.position).normalized * player.localScale.x;
        outParticle.transform.position = obj.transform.position;
        ParticleSystem.MainModule mainModule = outParticle.GetComponent<ParticleSystem>().main;
        mainModule.startColor = obj.GetComponent<SpriteRenderer>().color;
        outParticle.GetComponent<ParticleSystem>().Play();
        return outParticle;
    }
}
