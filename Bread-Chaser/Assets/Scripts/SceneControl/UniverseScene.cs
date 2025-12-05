using UnityEngine;

public class UniverseScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Universe;
        _bossId = (int)MonsterID.Velve;
        _mobid = (int)MonsterID.UniverseMob;
        Managers.Game.BossStagPointChanger(Managers.Game.BossBattleScoreCut / 2);

        //Managers.Sound.Play("BGM/Bagel Street (loop)", Define.Sound.Bgm);

        for (int i = 0; i < 3; i++)
        {
            if (i == 0)
                Managers.Area.SpawnArea(SceneName, ref Managers.Area.totalLength, 0, false, true);
            else
                Managers.Area.SpawnArea(SceneName, ref Managers.Area.totalLength, 0, true, true);
        }
    }
}
