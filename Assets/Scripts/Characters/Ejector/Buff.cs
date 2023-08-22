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

        //撞到玩家
        if (collision.gameObject.CompareTag("Player") && !isDisappeared)
        {
            ejectroStats.CurrentHealth--;
            //TODO 加文字提示

            //播放音效
            AudioManager.Instance.PlayGetBuffFX();
            Debug.Log("Buff+++");
        }

        //撞到障碍物
        if (collision.gameObject.CompareTag("Obstacle") && !isDisappeared)
        {
            ejectroStats.CurrentHealth--;
            ParticlePool.Instance.GetFromPool(this.gameObject);
        }
    }
}
