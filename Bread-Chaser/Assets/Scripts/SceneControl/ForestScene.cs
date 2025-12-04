using UnityEngine;

public class ForestScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Forest;
        _bossId = (int)MonsterID.ForestBoss;
        _mobid = (int)MonsterID.ForestMob;

        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
                Managers.Area.SpawnArea(SceneName, ref Managers.Area.totalLength, 0, false, true);
            else
                Managers.Area.SpawnArea(SceneName, ref Managers.Area.totalLength, 0, true, true);
        }
    }

    public override void ForestGimmick()
    {

    }
}
