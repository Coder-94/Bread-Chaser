using System;
using UnityEngine;

public class Stat : MonoBehaviour
{
    public Action<float> HpCountAction = null;

    protected Define.WorldObject objectType = Define.WorldObject.Unknown;

    public int Id { get; protected set; }
    public float Hp { get; protected set; }
    public float CurrentHp { get; protected set; }
    public float Atk { get; protected set; }


}
