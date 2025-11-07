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

   

    public float NormalAtkCalcul(float power, float coEfficient)
    {
        float random = Util.RandomCalculate(0.9f, 1.1f);
        float finalDmg = power * coEfficient * random;
        return finalDmg;
    }

    public float SkillCalcul(float power, float coEfficient)
    {
        /*
            Final = ATK × SkillBase × ( 1 + S * PlayerCoeff / (H + PlayerCoeff) )

            SkillBase : 스킬 고유 배수 (예: 2.5, 3.0, 4.0…)
            PlayerCoeff : 플레이어 스킬 배수(누적 강화량)
            S : 유저배수의 최대 추가 비율 (예: 1.0 → 최대 +100%)
            H : 절반 효과가 되는 지점(반감치). PlayerCoeff = H일 때 보너스는 S의 절반.
         */
        return power;
    }

    public float ShieldCalcul(float hp, float coEff)
    {
        float shield = hp* coEff;

        return shield;
    }


    
    public virtual void OnAttacked(float power) { }
    public virtual bool OnAttacked(float power, float coEfficient, ref bool targetNotDead) { return targetNotDead; }
    public virtual void OnPoisoned(float term, float power) { }
    public virtual void Shield() { }
}
