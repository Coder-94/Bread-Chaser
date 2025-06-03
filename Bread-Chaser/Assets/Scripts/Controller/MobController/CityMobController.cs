using UnityEngine;

public class CityMobController : NormalMobBase
{
    #region Unity Scripts
    protected void Update()
    {
        Idle();
        RotFixer(player);
    }

    private void LateUpdate()
    {
        PosFixer();
    }

    #endregion


}
