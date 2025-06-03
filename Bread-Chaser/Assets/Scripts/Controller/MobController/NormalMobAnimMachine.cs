using System.Collections.Generic;
using UnityEngine;

public class NormalMobAnimMachine : MonoBehaviour
{

    protected void Attack()
    {
        Vector3 spawnPos = transform.GetChild(0).position;
        GameObject bullet = Managers.Resource.Instantiate($"Entity/{gameObject.name}Bullet", null, 20);
        bullet.transform.position = spawnPos;
        bullet.transform.rotation = gameObject.transform.rotation;

        BulletController bulletController = bullet.GetComponent<BulletController>();
        bulletController.parentStat = GetComponent<NormalMobStat>();
    }
}
