using UnityEngine;

public class CityScene : BaseScene
{
    private void Update()
    {
        MobSpawner((int)MonsterID.CityMob, 5, 11);
    }

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.City;

        //Managers.Sound.Play("BGM/Bagel Street (loop)", Define.Sound.Bgm);

        for(int i=0; i<3; i++)
        {
            if(i==0)
                Managers.Area.SpawnArea(SceneName, ref Managers.Area.totalLength, 0, false, true);
            else
                Managers.Area.SpawnArea(SceneName, ref Managers.Area.totalLength, 0, true, true);
        }
    }

    public override void Clear()
    {
        //throw new System.NotImplementedException();
    }
}
