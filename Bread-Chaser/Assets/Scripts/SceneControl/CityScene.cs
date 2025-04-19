using UnityEngine;

public class CityScene : BaseScene
{

    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.City;

        for(int i=0; i<3; i++)
        {
            if(i==0)
                Managers.Area.SpawnArea(Managers.Scene.GetSceneName(Define.Scene.City),
                                        ref Managers.Area.totalLength, false);
            else
                Managers.Area.SpawnArea(Managers.Scene.GetSceneName(Define.Scene.City),
                                        ref Managers.Area.totalLength);
        }
    }

    void Update()
    {
        
    }

    public override void Clear()
    {
        //throw new System.NotImplementedException();
    }
}
