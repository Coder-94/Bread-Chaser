using Data;
using System.Collections;
using UnityEngine;

public class NormalMobController : BaseMobController
{
    enum State
    {
        IdleOpen,
        Atk,
        DeathSpellCast,
        Damaged,
        Die
    }

    #region variables
    private float                   _targetValue = 0f;
    private float                   _duration = 0.2f;

    private NormalMobStat           stat;

    private Vector3                 _spawnPos;
    #endregion

    #region Unity Scripts
    protected void OnEnable()
    {
        _spawnPos = gameObject.transform.position;
    }

    protected void Update()
    {
        OnUpdate(5.0f);
    }
    #endregion

    #region Initialize
    protected override void Init()
    {
        base.Init();

        stat = gameObject.GetOrAddComponent<NormalMobStat>();
    }
    #endregion

    protected override void Clear()
    {
        for(int i=0; i<Managers.Scene.CurrentScene.spawnedMobChecker.Length; i++)
        {
            if (_spawnPos == Managers.Scene.CurrentScene.spawnedMobChecker[i].spawnedPos)
            {
                Managers.Scene.CurrentScene.spawnedMobChecker[i].isEnable = false;
                Managers.Scene.CurrentScene.monsterCount--;
                break;
            }
        }
            
    }
}
