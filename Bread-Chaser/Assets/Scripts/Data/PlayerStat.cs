using System;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public Action<float> HpCountAction = null;

    public int      Level { get; private set; } = 1;
    public float    Hp { get; private set; }
    public float    CurrentHp { get; private set; }
    public int      Atk { get; private set; }
    public float    AtkSpd { get; private set; }

    public float    moveSpeed;

    private void Start()
    {
        Init();
    }

    void Init()
    {
        Data.PlayerStat stat = Managers.Data.PLStatDict[Level];

        Level = stat.level;
        Hp = stat.hp;
        CurrentHp = Hp;
        Atk = stat.atk;
        moveSpeed = stat.moveSpeed;
        AtkSpd = stat.atkSpeed;
    }

    public void OnAttacked(float power)
    {
        CurrentHp -= power;
        HpCountAction.Invoke(CurrentHp);
    }

    public void Heal(float healPower)
    {
        if (CurrentHp >= Hp)
            return;

        CurrentHp += healPower;

        if (CurrentHp > Hp)
            CurrentHp = Hp;

        HpCountAction.Invoke(CurrentHp);
    }
}
