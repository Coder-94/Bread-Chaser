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
        int currentGrade = int.Parse(name[name.Length - 1].ToString());

        Data.Stat stat = Managers.Data.StatDict[currentGrade];

        Hp = stat.hp;
        Atk = stat.atk;
    }
}
