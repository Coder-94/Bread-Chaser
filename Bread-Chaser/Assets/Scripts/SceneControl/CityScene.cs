using UnityEngine;

public class CityScene : BaseScene
{

    void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        Managers.Area.SpawnArea("City", true);
        SceneType = Define.Scene.City;
    }

    void Update()
    {
        
    }

    public override void Clear()
    {
        //throw new System.NotImplementedException();
    }
}
