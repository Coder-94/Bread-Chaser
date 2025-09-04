using UnityEngine;

public class NormalMobStat : Stat
{
    
    public int      AtkSpeed { get; private set; }

    public void SetID(int id)
    {
        Id = id;
        Init();
    }

    public bool OnAttacked(float power, ref bool targetNotDead)
    {
        Hp -= power;

        if (Hp <= 0)
            return targetNotDead = false;

        return targetNotDead = true;
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
        Hp = (int)stat.hp;
    }
}
