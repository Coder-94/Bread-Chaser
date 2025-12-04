using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MobAnimMachine : MonoBehaviour
{

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
        GameObject boom = Managers.Resource.Instantiate($"Entity/City/LocalExplosion", null, 1);
        boom.transform.position = spawnPos;

        LocalAtkController localAtk = boom.GetComponent<LocalAtkController>();
        if (localAtk != null)
        {
            localAtk.SetOwner(gameObject);
        }
    }

    //CityMob doesn't have any skills

    public void ForestMobSkill()
    {

    }

    public void ForestBossSkill()
    {

    }

    public void IcycleMobSkill()
    {

    }

    public void IcycleBossSkill()
    {

    }

    public void SpaceMobSkill()
    {

    }

    public void SpaceBossSkill()
    {

    }

    protected void ReturnToIdle()
    {
        GetComponent<MobController>().CurrentState = Define.NormalMobStatus.Idle;
    }
}
