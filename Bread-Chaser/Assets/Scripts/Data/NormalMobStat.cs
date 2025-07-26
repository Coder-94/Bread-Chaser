using UnityEngine;

public class NormalMobStat : MonoBehaviour
{
    public int      Id { get; private set; }
    public float    Hp { get; private set; }
    public int      Atk { get; private set; }
    public int      AtkSpeed { get; private set; }

    public void SetID(int id)
    {
        Id = id;
        Init();
    }

    public void OnAttacked(float power)
    {
        Hp -= power;
    }

    void Init()
    {
        Data.MobStat stat = Managers.Data.NMStatDict[Id];

        Hp = stat.hp;
        Atk = stat.atk;
        AtkSpeed = stat.atkSpeed;
    }

    public void Clear()
    {
        Data.MobStat stat = Managers.Data.NMStatDict[Id];
        Hp = stat.hp;
    }
}
