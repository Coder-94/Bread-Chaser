using System.Collections;
using UnityEngine;

public class NormalMobStat : Stat
{
    
    public int          AtkSpeed { get; private set; }

    private Coroutine   _poisonCoroutine;

    public void SetID(int id)
    {
        Id = id;
        Init();
    }

    public override bool OnAttacked(float power, float coEfficient, ref bool targetNotDead)
    {
        Hp -= NormalAtkCalcul(power, coEfficient); ;

        if (Hp <= 0)
            return targetNotDead = false;

        return targetNotDead = true;
    }

    public override void OnPoisoned(float term, float power)
    {
        if (_poisonCoroutine != null)
        {
            StopCoroutine(_poisonCoroutine);
        }

        _poisonCoroutine = StartCoroutine(PoisonProcessCoroutine(term, power));
    }

    private IEnumerator PoisonProcessCoroutine(float term, float power)
    {
        float duration = 3f;
        float elapsedTime = 0f;
        float tickDMG = 0.3f;

        GameObject effect = Managers.Resource.Instantiate("Effect/Bubbles", gameObject.transform);
        effect.GetComponent<ParticleSystem>().Play();
        try
        {
            while (elapsedTime < duration)
            {
                Hp -= power * tickDMG;

                float tickInterval = 1f / (1f + term * 0.2f);

                yield return new WaitForSeconds(tickInterval);

                elapsedTime += tickInterval;
            }
        }
        finally
        {
            if (effect != null)
                Managers.Resource.Destroy(effect);

            _poisonCoroutine = null;
        }
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
