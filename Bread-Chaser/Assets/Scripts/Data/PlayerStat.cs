using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class StatEvolutionData
{
    public float IncreasedValue { get; set; } = 0; // 누적 성장치
    public bool FirstEvolve { get; set; } = false;  // 1차 진화 여부
    public bool SecondEvolve { get; set; } = false; // 2차 진화 여부
}

public class PlayerStat : Stat
{
    public int Level { get; private set; }
    public float AtkCoefficient { get; private set; }
    public float SkillCoefficient { get; private set; }
    public float FirstCap { get; private set; } = 0;
    public float SecondCap { get; private set; } = 0;
    public float MoveSpeed { get; private set; } = 0;
    public bool  LastEvolved { get; private set; } = false;
    public float CoolTime { get; private set; } = 0;
    public Define.IncreaseAbleStat EvolvedType { get; private set; } = Define.IncreaseAbleStat.Default;

    public bool IsSkillCool { get; private set; } = false;
    public float        BarrierCool { get; private set; } = 0;
    private float       _currentShieldHealth;
    private GameObject  _shieldEffectInstance;

    public Dictionary<Define.IncreaseAbleStat, StatEvolutionData> EvolutionData { get; private set; }

    private void Start()
    {
        Init();
    }

    void Init()
    {
        DontDestroyOnLoad(gameObject);
        Id = 0;

        Data.PlayerStat stat = Managers.Data.PLStatDict[Id];
        Level = stat.level;
        Hp = stat.hp;
        CurrentHp = Hp;
        Atk = stat.atk;
        AtkCoefficient = stat.atkCoefficient;
        SkillCoefficient = stat.skillCoefficient;
        BarrierCool = stat.barrierCool;
        FirstCap = stat.firstCap;
        SecondCap = stat.secondCap;
        MoveSpeed = stat.moveSpeed;

        EvolutionData = new Dictionary<Define.IncreaseAbleStat, StatEvolutionData>
        {
            { Define.IncreaseAbleStat.Atk,      new StatEvolutionData() },
            { Define.IncreaseAbleStat.MoveSpd,  new StatEvolutionData() },
            { Define.IncreaseAbleStat.Hp,       new StatEvolutionData() },
            { Define.IncreaseAbleStat.SkillDMG, new StatEvolutionData() }
        };
    }

    public void SetStat(Define.IncreaseAbleStat stat)
    {
        Level++;
        float increaseRange = Util.RandomCalculate(0.2f, 0.5f);
        StatEvolutionData data = EvolutionData[stat];
        data.IncreasedValue += increaseRange;

        switch (stat)
        {
            case Define.IncreaseAbleStat.Atk: Atk += increaseRange; break;
            case Define.IncreaseAbleStat.MoveSpd: MoveSpeed += increaseRange; break;
            case Define.IncreaseAbleStat.Hp: Hp += increaseRange; break;
            case Define.IncreaseAbleStat.SkillDMG: SkillCoefficient += increaseRange; break;
        }

        Evolving(stat);
    }

    public void BuffStat(Define.IncreaseAbleStat stat, float value)
    {
        switch (stat)
        {
            case Define.IncreaseAbleStat.Atk: Atk = value; break;
            case Define.IncreaseAbleStat.MoveSpd: MoveSpeed += value; break;
            case Define.IncreaseAbleStat.Hp: Hp += value; break;
            case Define.IncreaseAbleStat.SkillDMG: SkillCoefficient += value; break;
        }
    }

    #region evolve
    void Evolving(Define.IncreaseAbleStat stat)
    {
        StatEvolutionData data = EvolutionData[stat];

        if (!data.FirstEvolve)
        {
            if (data.IncreasedValue >= FirstCap)
            {
                data.FirstEvolve = true;

                if (EvolutionData[Define.IncreaseAbleStat.Atk].FirstEvolve)
                    AtkCoefficient = 0.8f;

                if (EvolutionData[Define.IncreaseAbleStat.Hp].FirstEvolve)
                    Shield();
            }

            if (EvolutionData[Define.IncreaseAbleStat.SkillDMG].FirstEvolve)
                gameObject.GetComponent<PlayerController>().PunchEffectNull();
        }
        else if (!LastEvolved && !data.SecondEvolve)
        {
            if (data.IncreasedValue >= SecondCap)
                data.SecondEvolve = true;

            LastEvolved = true;

            EvolvedType = stat;

            switch (EvolvedType)
            {
                case Define.IncreaseAbleStat.Atk: CoolTime = 15; break;
                case Define.IncreaseAbleStat.MoveSpd: CoolTime = 0; break;
                case Define.IncreaseAbleStat.Hp: CoolTime = 0; break;
                case Define.IncreaseAbleStat.SkillDMG: CoolTime = 0; break;
            }

            GameObject.Find("SkillBtn").GetComponent<SkillBtn>().SkillInit(EvolvedType);
        }
    }


    public void Test(int a) 
    {
        if (a == 0)
        {
            Debug.Log("1");
            EvolutionData[Define.IncreaseAbleStat.Atk].FirstEvolve = true;
        }
        else if (a == 1)
        {
            EvolutionData[Define.IncreaseAbleStat.Atk].SecondEvolve = true;
            CoolTime = 15;
            Debug.Log(CoolTime);
            GameObject.Find("SkillBtn").GetComponent<SkillBtn>().SkillInit(Define.IncreaseAbleStat.Atk);
        }
    }

    #region shield
    public override void Shield() { StartCoroutine(ShieldCreate()); }

    IEnumerator ShieldCreate()
    {
        if (_currentShieldHealth <= 0)
        {
            ShieldCreator();
        }

        while (true)
        {
            float interval = BarrierCool;

            if (EvolutionData[Define.IncreaseAbleStat.Hp].SecondEvolve)
            {
                interval -= 5;
            }

            yield return new WaitForSeconds(interval);

            if (_currentShieldHealth <= 0)
            {
                ShieldCreator();
            }
        }
    }

    void ShieldCreator()
    {
        float shieldSize = 0;
        if (EvolutionData[Define.IncreaseAbleStat.Hp].SecondEvolve)
        {
            shieldSize = ShieldCalcul(Hp, 0.48f);
        }
        else if (EvolutionData[Define.IncreaseAbleStat.Hp].FirstEvolve)
        {
            shieldSize = ShieldCalcul(Hp, 0.18f);
        }

        _currentShieldHealth = shieldSize;

        if (_shieldEffectInstance == null)
        {
            _shieldEffectInstance = Managers.Resource.Instantiate("Effect/BluePolygonShield", gameObject.transform);
        }
    }
    #endregion

    #region active skill
    public void SkillOpen(Define.IncreaseAbleStat evolvedStat)
    {
        if (!IsSkillCool) 
        {
            GameObject skill = Managers.Resource.Instantiate($"Effect/Skill/{evolvedStat}Skill");

            if (CoolTime > 0)
            {
                StartCoroutine(CooldownCoroutine(CoolTime));
            }
        }
    }

    private IEnumerator CooldownCoroutine(float duration)
    {
        IsSkillCool = true;

        yield return new WaitForSeconds(duration);

        IsSkillCool = false;
    }
    #endregion

    #endregion

    public override void OnAttacked(float power)
    {
        if (_currentShieldHealth > 0)
        {
            float damageToShield = Mathf.Min(power, _currentShieldHealth);
            _currentShieldHealth -= damageToShield;
            power -= damageToShield;

            if (_currentShieldHealth <= 0)
            {
                if (_shieldEffectInstance != null)
                {
                    Managers.Resource.Destroy(_shieldEffectInstance);
                    _shieldEffectInstance = null;
                }
            }
        }

        if (power > 0)
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
