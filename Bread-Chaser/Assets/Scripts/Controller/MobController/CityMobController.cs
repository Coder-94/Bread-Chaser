using UnityEngine;

public class CityMobController : NormalMobBase
{
    #region Unity Scripts
    protected void Update()
    {
        Idle();
        RotFixer(player);

        NormalMobStat stat = GetComponent<NormalMobStat>();
        if (stat.Hp == 0)
            Managers.Resource.Destroy(gameObject);
    }

    private void LateUpdate()
    {
        PosFixer();
    }

    #endregion


}
