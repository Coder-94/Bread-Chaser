using UnityEngine;

public class IceLandScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Forest;
        _bossId = (int)MonsterID.IcycleBoss;
        _mobid = (int)MonsterID.IcycleMob;

        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
                Managers.Area.SpawnArea(SceneName, ref Managers.Area.totalLength, 0, false, true);
            else
                Managers.Area.SpawnArea(SceneName, ref Managers.Area.totalLength, 0, true, true);
        }
    }
}
