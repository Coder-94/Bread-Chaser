using UnityEngine;

public class NormalMobStat : MonoBehaviour
{
    public int Hp { get; private set; }
    public int Atk { get; private set; }
    public int AtkSpeed { get; private set; }

    private void Start()
    {
        Init();
    }

    void Init()
    {
        Data.NormalMobStat stat = Managers.Data.NMStatDict[Managers.Scene.CurrentScene.SceneName];

        Hp = stat.hp;
        Atk = stat.atk;
        AtkSpeed = stat.atkSpeed;
    }
}
