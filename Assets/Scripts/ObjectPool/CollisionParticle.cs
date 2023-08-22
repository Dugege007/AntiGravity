using UnityEngine;

public class CollisionParticle : MonoBehaviour
{
    private Transform player;
    private ParticleSystem particle;

    [Header("Time")]
    //开始显示的时间
    public float activeStart;
    //显示时间
    public float activeTime;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        particle = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        activeStart = Time.time;
    }

    public void CreateParticle(GameObject obj)
    {
        transform.position = player.position + (obj.transform.position - player.position).normalized * player.localScale.x;
        //particle.textureSheetAnimation.SetSprite(0, obj.GetComponent<SpriteRenderer>().sprite);
        ParticleSystem.MainModule mainModule = particle.main;
        mainModule.startColor = obj.GetComponent<SpriteRenderer>().color;
        particle.Play();
    }

    private void Update()
    {
        if (Time.time >= activeStart + activeTime)
        {
            //返回对象池
            ParticlePool.Instance.ReturnPool(this.gameObject);
        }
    }
}
