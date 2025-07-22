using Data;
using UnityEngine;

public class CityMobController : NormalMobBase
{
    #region Unity Scripts
    protected void Update()
    {
        Idle();
        RotFixer(player);
        
        if (mobStat.Hp <= 0)
            Clear();
    }

    private void LateUpdate()
    {
        PosFixer();
    }

    #endregion


}
