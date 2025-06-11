using UnityEngine;

public class PlayerAnimMachine : MonoBehaviour
{
    void BooleanInit()
    {
        PlayerStat stat = GetComponent<PlayerStat>();

        stat.isAtk = false;
    }
}
