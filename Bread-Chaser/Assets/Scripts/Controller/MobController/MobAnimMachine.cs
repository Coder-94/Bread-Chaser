using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MobAnimMachine : MonoBehaviour
{
    public bool SkillUsing { private set; get; } = false;
    
    public bool LastBossSkillUsing { private set; get; } = false;
    private Coroutine _channelingCoroutine;

    protected void Attack()
    {
        Vector3 spawnPos = transform.GetChild(0).position;
        Managers.Sound.Play("SE/Shoot");
        GameObject bullet = Managers.Resource.Instantiate($"Entity/{Managers.Scene.CurrentScene.SceneName}/{gameObject.name}Bullet", gameObject.transform, 20);
        bullet.transform.position = spawnPos;
        bullet.transform.rotation = gameObject.transform.rotation;

        BulletController bulletController = bullet.GetComponent<BulletController>();
        if (bulletController != null)
        {
            bulletController.SetShooter(gameObject);
        }
    }

    protected void LocalAtk() 
    {
        Vector3 spawnPos = GetComponent<MobController>().targetedPos.transform.position;
        GameObject boom = Managers.Resource.Instantiate($"Entity/{Managers.Scene.CurrentScene.SceneName}/LocalExplosion", null, 1);
        boom.transform.position = spawnPos;

        Managers.Sound.Play("SE/Boom");
        LocalAtkController localAtk = boom.GetComponent<LocalAtkController>();
        if (localAtk != null)
        {
            localAtk.SetOwner(gameObject);
        }
    }

    //CityMob doesn't have any skills


    public void ForestBossSkill()
    {
        SkillUsing = true;
        GameObject go = Managers.Resource.Instantiate("Effect/EnemySkill/ForestSkill");
        go.GetComponent<ForestSkill>().Init(SkillTogle, gameObject);
    }


    public void IcycleBossSkill()
    {
        SkillUsing = true;
        GameObject go = Managers.Resource.Instantiate("Effect/EnemySkill/IceLandSkill");
        go.GetComponent<IceLandSkill>().Init(SkillTogle, gameObject);
    }

    protected void SpaceBossAttack()
    {
        int Random = UnityEngine.Random.Range(0, 2);
        if (Random == 0)
            ForestBossSkill();
        else
            IcycleBossSkill();
    }

    public void SpaceBossSkill()
    {
        if (LastBossSkillUsing) return;

        LastBossSkillUsing = true;
        _channelingCoroutine = StartCoroutine(SpaceBossChanneling());
    }

    IEnumerator SpaceBossChanneling()
    {
        int maxChargeCount = 4;
        float interval = 4.0f;
        float finalAttackDelay = 1.0f;

        GameObject spawnParent = transform.GetChild(0).gameObject;
        

        for (int i = 0; i < maxChargeCount; i++)
        {
            if (!LastBossSkillUsing) yield break;

            GameObject charger = Managers.Resource.Instantiate("Effect/EnemySkill/PurpleCharge");
            if (charger != null)
            {
                Managers.Sound.Play("SE/Charge");
                charger.transform.parent = spawnParent.transform;
                charger.transform.localPosition = Vector3.zero;
            }
            yield return new WaitForSeconds(interval);
        }

        if (LastBossSkillUsing)
        {
            yield return new WaitForSeconds(finalAttackDelay);
        }

        if (LastBossSkillUsing)
        {
            SpawnVelveBoom(true);

            LastBossSkillUsing = false;
        }

    }

    private void SpawnVelveBoom(bool isSuccess)
    {
        Vector3 targetPos = Vector3.zero;

        if (isSuccess)
        {
            GameObject player = Managers.Game.GetPlayer();
            if (player != null) targetPos = player.transform.position;
        }
        else
        {
            targetPos = transform.position;
        }

        GameObject boom = Managers.Resource.Instantiate("Effect/EnemySkill/VelveBoom");
        if (boom != null)
        {
            boom.transform.position = targetPos;

            VelveBoomSkill boomScript = boom.GetComponent<VelveBoomSkill>();
            if (boomScript != null)
            {
                boomScript.Init(isSuccess);
            }
        }
    }


    public void BreakSpaceBossSkill()
    {
        if (LastBossSkillUsing)
        {
            LastBossSkillUsing = false;

            if (_channelingCoroutine != null)
                StopCoroutine(_channelingCoroutine);

            SpawnVelveBoom(false);
        }
    }

    protected void ReturnToIdle()
    {
        GetComponent<MobController>().CurrentState = Define.NormalMobStatus.Idle;
    }

    public void SkillTogle()
    {
        if(SkillUsing)
            SkillUsing = false;
    }
}
