using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : Singleton<ParticlePool>
{
    public GameObject particlePrefab;
    public int particleCount;

    //����ض���
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

            //ȡ�����ã����ض����
            ReturnPool(newParticle);
        }
    }

    public void ReturnPool(GameObject obj)
    {
        //obj.gameObject.SetActive(false);

        //��ӵ�����ĩ��
        availableObjects.Enqueue(obj);
    }

    public GameObject GetFromPool(GameObject obj)
    {
        if (availableObjects.Count == 0)
        {
            FillPool();
        }
        GameObject outParticle = availableObjects.Dequeue();
        outParticle.gameObject.SetActive(true);  // �� SetActive(true) ʱ����ִ��һ��OnEnable()
        //transform.position = player.position + (obj.transform.position - player.position).normalized * player.localScale.x;
        outParticle.transform.position = obj.transform.position;
        ParticleSystem.MainModule mainModule = outParticle.GetComponent<ParticleSystem>().main;
        mainModule.startColor = obj.GetComponent<SpriteRenderer>().color;
        outParticle.GetComponent<ParticleSystem>().Play();
        return outParticle;
    }
}
