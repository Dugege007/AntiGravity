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
            //TODO 加碰撞特效、文字提示、相机抖动

            //播放音效
            AudioManager.Instance.PlayGetHitFX();
            Debug.Log("Buff---");
        }
    }
}