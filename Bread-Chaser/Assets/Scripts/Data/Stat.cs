using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Stat : MonoBehaviour
{
    public Action<float> HpCountAction = null;

    public int Id { get; protected set; }
    public float Hp { get; protected set; }
    public float CurrentHp { get; protected set; }
    public float Atk { get; protected set; }

    protected GameObject posionBubble = null;

    public float NormalAtkCalcul(float power, float coEfficient)
    {
        float random = Util.RandomCalculate(0.9f, 1.1f);
        float finalDmg = power * coEfficient * random;
        return finalDmg;
    }


    public float ShieldCalcul(float hp, float coEff)
    {
        float shield = hp* coEff;

        return shield;
    }

    public void RightKill() { Destroy(gameObject); }

    public void TestKill() { CurrentHp = 0; }

    public virtual void OnPlAttacked(GameObject attacker) { }

    public virtual bool OnEnemAttacked(GameObject attacker, float damageMultiplier = 1.0f) { return true; }

    public virtual bool OnBossAttacked() { return true; }

    public virtual bool OnSkillAttacked(GameObject attacker, float finalDamage) { return true; }

    public virtual void OnPoisoned(GameObject player) { }

    public virtual void OnSkillPoisoned(GameObject player) { }

    public virtual void Shield() { }
}
