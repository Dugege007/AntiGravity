using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : EjectorController
{
    protected override void OnEnable()
    {
        base.OnEnable();

    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.CompareTag("Player") && !isDisappeared)
        {
            //TODO ����ײ��Ч��������ʾ���������

            //������Ч
            AudioManager.Instance.PlayGetHitFX();
            Debug.Log("Buff---");
        }
    }
}