using UnityEngine;

public class NormalMobStat : MonoBehaviour
{
    public int Hp { get; private set; }
    public int Atk { get; private set; }

    private void Start()
    {
        Init();
    }

    void Init()
    {
        Data.Stat stat = Managers.Data.StatDict[Managers.Scene.CurrentScene.SceneName];

        Hp = stat.hp;
        Atk = stat.atk;
    }
}
