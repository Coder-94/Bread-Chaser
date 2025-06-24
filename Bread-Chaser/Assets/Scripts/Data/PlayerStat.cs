using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public int      Level { get; private set; } = 1;
    public int      Hp { get; private set; }
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
        Atk = stat.atk;
        moveSpeed = stat.moveSpeed;
        AtkSpd = stat.atkSpeed;
    }
}
