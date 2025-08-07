using Data;
using UnityEngine;

public class CityMobController : NormalMobBase
{
    #region Unity Scripts
    protected void Update()
    {
        //dd
        if (mobStat.Hp <= 0)
            Clear();

        RotFixer(player);

        switch (myState)
        {
            case Define.NormalMobStatus.Idle:
                Attack();
                break;
        }
    }

    private void LateUpdate()
    {
        PosFixer();
    }

    #endregion


}
