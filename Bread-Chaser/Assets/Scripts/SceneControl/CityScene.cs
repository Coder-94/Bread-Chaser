using UnityEngine;

public class CityScene : BaseScene
{
    private void Update()
    {
        MobSpawner(5, 11);
    }

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.City;
        for(int i=0; i<3; i++)
        {
            if(i==0)
                Managers.Area.SpawnArea(SceneName, ref Managers.Area.totalLength, false);
            else
                Managers.Area.SpawnArea(SceneName, ref Managers.Area.totalLength);
        }
    }

    public override void Clear()
    {
        //throw new System.NotImplementedException();
    }
}
