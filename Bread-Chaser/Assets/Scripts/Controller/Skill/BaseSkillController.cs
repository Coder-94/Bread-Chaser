using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class BaseSkillController : MonoBehaviour
{
    protected PlayerStat            stat;
    protected float                 duration;
    protected List<Stat>            hitTargets = new List<Stat>();
    protected bool                  hitOncePerTarget = false;
    protected CameraController      cam;
    protected ParticleSystem        particle;

    protected virtual void OnEnable()
    {
        hitTargets.Clear();

        if(particle == null )
            particle = GetComponent<ParticleSystem>();

        if (stat == null)
            stat = Managers.Game.GetPlayer().GetComponent<PlayerStat>();

        particle.Play();
    }

    protected void Start()
    {
        Init();
    }

    protected virtual void Init() 
    { 
        if (stat == null) 
            stat = Managers.Game.GetPlayer().GetComponent<PlayerStat>(); 
        cam = Camera.main.GetComponent<CameraController>();

        StartCoroutine(DespawnTimer());
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == (int)Define.Layer.Enemy || other.gameObject.layer == (int)Define.Layer.Boss)
        {
            Stat enemyStat = other.GetComponent<Stat>();

            if (enemyStat != null)
            {
                if (hitOncePerTarget && hitTargets.Contains(enemyStat))
                {
                    return;
                }

                float damage = CalculateDamage();

                if (damage > 0)
                {
                    enemyStat.OnSkillAttacked(stat.gameObject, damage);
                }

                OnHitAdditionalEffect(enemyStat);

                if (hitOncePerTarget)
                {
                    hitTargets.Add(enemyStat);
                }
            }
        }
    }

    protected IEnumerator DespawnTimer()
    {
        yield return new WaitForSeconds(duration);

        Managers.Resource.Destroy(gameObject);
    }

    protected virtual void OnHitAdditionalEffect(Stat enemyStat) { }

    protected abstract float CalculateDamage();
}
