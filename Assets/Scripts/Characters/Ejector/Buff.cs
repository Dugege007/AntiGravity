using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : EjectorController
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        //ײ�����
        if (collision.gameObject.CompareTag("Player") && !isDisappeared)
        {
            ejectroStats.CurrentHealth--;
            //TODO ��������ʾ

            //������Ч
            AudioManager.Instance.PlayGetBuffFX();
            Debug.Log("Buff+++");
        }

        //ײ���ϰ���
        if (collision.gameObject.CompareTag("Obstacle") && !isDisappeared)
        {
            ejectroStats.CurrentHealth--;
            ParticlePool.Instance.GetFromPool(this.gameObject);
        }
    }
}
