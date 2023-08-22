using UnityEngine;

public class ShadowSprite : MonoBehaviour
{
    private Transform player;
    private SpriteRenderer thisSprite;
    private SpriteRenderer playerSprite;
    private Color color;

    [Header("Time")]
    //开始显示的时间
    public float activeStart;
    //显示时间
    public float activeTime;

    [Header("Alpha")]
    private float alpha;
    //初始值
    public float alphaSet;
    public float alphaMultiplier;

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        thisSprite = GetComponent<SpriteRenderer>();
        playerSprite = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        thisSprite.sprite = playerSprite.sprite;
        transform.position = player.position;
        transform.localScale = player.localScale;
        transform.rotation = player.rotation;

        activeStart = Time.time;
    }

    private void Update()
    {
        //alpha = Mathf.Max(alphaMultiplier - Time.deltaTime, 0);
        alpha *= alphaMultiplier;
        color = new Color(1, 1, 1, alpha);
        thisSprite.color = color;

        if (Time.time >= activeStart + Mathf.Min(PlayerController.Instance.playerStats.MaxMoveTime, activeTime))
        {
            //返回对象池
            ShadowPool.Instance.ReturnPool(this.gameObject);
        }
    }
}
