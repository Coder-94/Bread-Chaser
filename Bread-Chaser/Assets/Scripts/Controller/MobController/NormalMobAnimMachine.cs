using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NormalMobAnimMachine : MonoBehaviour
{

    protected void Attack()
    {
        Vector3 spawnPos = transform.GetChild(0).position;
        GameObject bullet = Managers.Resource.Instantiate($"Entity/{Managers.Scene.CurrentScene.SceneName}/{gameObject.name}Bullet", null, 20);
        bullet.transform.position = spawnPos;
        bullet.transform.rotation = gameObject.transform.rotation;

        BulletController bulletController = bullet.GetComponent<BulletController>();
        bulletController.parentStat = GetComponent<NormalMobStat>();
    }

    protected void LocalAtk() 
    {
        Vector3 spawnPos = GetComponent<NormalMobBase>().targetedPos.transform.position;
        GameObject boom = Managers.Resource.Instantiate($"Entity/City/LocalExplosion", null, 1);
        boom.transform.position = spawnPos;
    }

    protected void ReturnToIdle()
    {
        GetComponent<NormalMobBase>().CurrentState = Define.NormalMobStatus.Idle;
    }
}
