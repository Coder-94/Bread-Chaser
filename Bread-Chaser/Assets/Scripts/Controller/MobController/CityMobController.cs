using Data;
using UnityEngine;

public class CityMobController : NormalMobBase
{
    protected override void Init()
    {
        base.Init();
        _spAtkToggle = true;
    }

    protected override void SpecialAtk()
    {
        
    }
}
