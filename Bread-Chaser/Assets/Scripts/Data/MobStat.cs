using System.Collections;
using UnityEngine;

public class MobStat : Stat
{
    
    public int          AtkSpeed { get; private set; }
    public float        Exp { get; private set; }
    private Coroutine   _poisonCoroutine;
    private Coroutine   _skillPoisonCoroutine;
    MobController       _control;

    public void SetID(int id)
    {
        Id = id;
        Init();
    }


    public override bool OnEnemAttacked(GameObject attacker, float damageMultiplier = 1.0f)
    {
        PlayerStat attackerStat = attacker.GetComponent<PlayerStat>();

        float damage = NormalAtkCalcul(attackerStat.Atk, attackerStat.AtkCoefficient);

        float finalDamage = damage * damageMultiplier;

        CurrentHp -= finalDamage;

        HpCountAction?.Invoke(CurrentHp);

        if (CurrentHp <= 0) return false;

        return true;
    }

    public bool VelveFailure()
    {
        int hitsToKill = 7;

        float damage = (Hp / (float)hitsToKill) * 1.5f;
        CurrentHp -= damage;

        HpCountAction?.Invoke(CurrentHp);

        if (CurrentHp <= 0) return false;

        return true;
    }

    public override bool OnBossAttacked()
    {
        int hitsToKill = 7;

        float damage = Hp / (float)hitsToKill;
        CurrentHp -= damage;

        HpCountAction?.Invoke(CurrentHp);

        MobAnimMachine anim = GetComponent<MobAnimMachine>();
        if (anim != null && anim.LastBossSkillUsing)
        {
            anim.BreakSpaceBossSkill();
        }

        if (CurrentHp <= 0) return false;

        return true;
    }

    public override bool OnSkillAttacked(GameObject attacker, float finalDamage)
    {
        CurrentHp -= finalDamage;

        Managers.Sound.Play("SE/HardHit");

        HpCountAction?.Invoke(CurrentHp);
        if (CurrentHp <= 0) return false;
        return true;
    }

    public override void OnPoisoned(GameObject player)
    {
        if (_poisonCoroutine != null)
        {
            StopCoroutine(_poisonCoroutine);
        }

        _poisonCoroutine = StartCoroutine(PoisonProcessCoroutine(player));
    }

    private IEnumerator PoisonProcessCoroutine(GameObject player)
    {
        PlayerStat plStat = player.GetComponent<PlayerStat>();
        float duration = 2.5f;
        float elapsedTime = 0f;
        float tickDMG = plStat.Atk * 0.3f;

        SkinnedMeshRenderer renderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        Vector3 spawnPosition = renderer.bounds.center;

        posionBubble = Managers.Resource.Instantiate("Effect/Bubbles");
        posionBubble.transform.position = spawnPosition;
        posionBubble.transform.SetParent(gameObject.transform);

        posionBubble.GetComponent<ParticleSystem>().Play();
        try
        {
            while (elapsedTime < duration)
            {
                CurrentHp -= tickDMG;
                HpCountAction?.Invoke(CurrentHp);
                float tickInterval = 1f / (1f + plStat.MoveSpeed * 0.2f);

                yield return new WaitForSeconds(tickInterval);

                elapsedTime += tickInterval;
            }
        }
        finally
        {
            if (posionBubble != null)
                Managers.Resource.Destroy(posionBubble);

            _poisonCoroutine = null;
        }
    }

    public override void OnSkillPoisoned(GameObject player)
    {
        if (_skillPoisonCoroutine != null)
        {
            StopCoroutine(_skillPoisonCoroutine);
        }
        _skillPoisonCoroutine = StartCoroutine(SkillPoisonCoroutine(player));
    }

    private IEnumerator SkillPoisonCoroutine(GameObject player)
    {
        PlayerStat plStat = player.GetComponent<PlayerStat>();
        float duration = 1.5f;
        float elapsedTime = 0f;
        float tickDMG = plStat.SkillCoefficient * 0.8f;

        SkinnedMeshRenderer renderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        Vector3 spawnPosition = renderer.bounds.center;

        posionBubble = Managers.Resource.Instantiate("Effect/Bubbles");
        posionBubble.transform.position = spawnPosition;
        posionBubble.transform.SetParent(gameObject.transform);

        posionBubble.GetComponent<ParticleSystem>().Play();
        try
        {
            while (elapsedTime < duration)
            {
                CurrentHp -= tickDMG;
                HpCountAction?.Invoke(CurrentHp);
                float tickInterval = 1f / (1f + plStat.MoveSpeed * 0.35f);

                yield return new WaitForSeconds(tickInterval);

                elapsedTime += tickInterval;
            }
        }
        finally
        {
            if (posionBubble != null)
                Managers.Resource.Destroy(posionBubble);

            _poisonCoroutine = null;
        }
    }

    void Init()
    {
        Data.MobStat stat = Managers.Data.NMStatDict[Id];

        Hp = stat.hp;
        CurrentHp = Hp;
        Atk = stat.atk;
        AtkSpeed = stat.atkSpeed;
        Exp = stat.exp;

        _control = GetComponent<MobController>();
    }

    public void Clear()
    {
        if (_poisonCoroutine != null) StopCoroutine(_poisonCoroutine);
        if (_skillPoisonCoroutine != null) StopCoroutine(_skillPoisonCoroutine);
        _poisonCoroutine = null;
        _skillPoisonCoroutine = null;

        if (posionBubble != null)
        {
            Managers.Resource.Destroy(posionBubble);
            posionBubble = null;
        }

        Data.MobStat stat = Managers.Data.NMStatDict[Id];
        CurrentHp = Hp;

        Managers.Resource.Destroy(posionBubble);
        posionBubble = null;
        _poisonCoroutine = null;

    }
}
