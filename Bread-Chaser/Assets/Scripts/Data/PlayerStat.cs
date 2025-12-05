using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class StatEvolutionData
{
    public float IncreasedValue { get; set; } = 0;
    public bool FirstEvolve { get; set; } = false; 
    public bool SecondEvolve { get; set; } = false;
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
    public bool IsAtkBuffed { get; private set; } = false;
    public Define.IncreaseAbleStat EvolvedType { get; private set; } = Define.IncreaseAbleStat.Default;

    public event Action<Define.IncreaseAbleStat> OnFirstEvolved;
    public event Action<Define.IncreaseAbleStat, float> OnSecondEvolved;

    public float                BarrierCool { get; private set; } = 0;
    private float               _currentShieldHealth;
    private GameObject          _shieldEffectInstance;
    public float                CurrentShield => _currentShieldHealth;
    public float                MaxShield { get; private set; } = 0.001f;

    public bool                 IsInvincible { get; private set; } = false;
    private float               _hpDecayAmount = 0.1f;
    private float               _invincibleTime = 2f;
    private PlayerController    _controller;
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

        _controller = GetComponent<PlayerController>();
        StartCoroutine(HpDecayCoroutine());
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

    public void BuffStat(Define.IncreaseAbleStat stat, float value, bool isBuffActive)
    {
        if (stat == Define.IncreaseAbleStat.Atk)
        {
            Atk = value;
            IsAtkBuffed = isBuffActive;
        }
    }

    public void SetStat(Define.IncreaseAbleStat stat, float value)
    {
        switch (stat)
        {
            case Define.IncreaseAbleStat.Atk: Atk = value; break;
            case Define.IncreaseAbleStat.MoveSpd: MoveSpeed = value; break;
            case Define.IncreaseAbleStat.Hp: Hp += value; break;
            case Define.IncreaseAbleStat.SkillDMG: SkillCoefficient += value; break;
        }
    }

    #region evolve
    void Evolving(Define.IncreaseAbleStat stat)
    {
        StatEvolutionData data = EvolutionData[stat];

        if (!data.FirstEvolve && data.IncreasedValue >= FirstCap)
        {
            data.FirstEvolve = true;

            if (stat == Define.IncreaseAbleStat.Atk)
                AtkCoefficient = 0.8f;

            if (stat == Define.IncreaseAbleStat.Hp)
                Shield();

            OnFirstEvolved?.Invoke(stat);
        }
        else if (data.FirstEvolve && !LastEvolved && !data.SecondEvolve && data.IncreasedValue >= SecondCap)
        {
            data.SecondEvolve = true;
            LastEvolved = true;
            EvolvedType = stat;

            float coolTime = 0;
            switch (EvolvedType)
            {
                case Define.IncreaseAbleStat.Atk: coolTime = 12; break;
                case Define.IncreaseAbleStat.MoveSpd: coolTime = 10; break;
                case Define.IncreaseAbleStat.Hp: coolTime = 16; break;
                case Define.IncreaseAbleStat.SkillDMG: coolTime = 14; break;
            }

            OnSecondEvolved?.Invoke(EvolvedType, coolTime);
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

        MaxShield = shieldSize;
        _currentShieldHealth = shieldSize;

        if (_shieldEffectInstance == null)
        {
            _shieldEffectInstance = Managers.Resource.Instantiate("Effect/GoldPolygonShield", gameObject.transform);
        }
    }
    #endregion

    

    #endregion

    public void TrueDmgAttacked(float power = 1.0f)
    {
        if (power > 0)
        {
            CurrentHp -= power;
            HpCountAction?.Invoke(CurrentHp);

            if (CurrentHp > 0)
            {
                StartCoroutine(InvincibleProcess(true, _invincibleTime));

                if (_controller != null)
                {
                    if (_controller.CurrentState == Define.PlayerStatus.Running || _controller.CurrentState == Define.PlayerStatus.Attack || _controller.CurrentState == Define.PlayerStatus.Attacking)
                        _controller.CurrentState = Define.PlayerStatus.Damaged;
                    else
                        Managers.Sound.Play("SE/Hit");
                }
            }
        }
    }

    public override void OnPlAttacked(GameObject enemy, float duration = 1.0f)
    {
        if (IsInvincible) return;

        if (GetComponent<PlayerController>().CurrentState == Define.PlayerStatus.BossAtk || GetComponent<PlayerController>().CurrentState == Define.PlayerStatus.BossKeepAtk)
            return;

        if (_controller != null)
        {
            Define.PlayerStatus state = _controller.CurrentState;

            if (state == Define.PlayerStatus.Channeling ||  state == Define.PlayerStatus.Attacking)
            {
                return;
            }
        }

        Stat enemyStat = enemy.GetComponent<Stat>();
        float power = 0; 
        
        if(enemyStat != null)
            power = enemyStat.Atk;
        else
            power = 1f;

        power *= duration;

        if (_currentShieldHealth > 0)
        {
            float damageToShield = Mathf.Min(power, _currentShieldHealth);
            _currentShieldHealth -= damageToShield;
            power -= damageToShield;

            if (_currentShieldHealth <= 0)
            {
                if (_shieldEffectInstance != null)
                {
                    _currentShieldHealth = 0;
                    Managers.Resource.Destroy(_shieldEffectInstance);
                    _shieldEffectInstance = null;
                }
            }
        }

        if (power > 0)
        {
            CurrentHp -= power;
            HpCountAction?.Invoke(CurrentHp);

            if (CurrentHp > 0)
            {
                StartCoroutine(InvincibleProcess(true, _invincibleTime));

                if (_controller != null)
                {
                    if(_controller.CurrentState == Define.PlayerStatus.Running || _controller.CurrentState == Define.PlayerStatus.Attack || _controller.CurrentState == Define.PlayerStatus.Attacking)
                        _controller.CurrentState = Define.PlayerStatus.Damaged;
                    else
                        Managers.Sound.Play("SE/Hit");
                }
            }
        }
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


    private IEnumerator HpDecayCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);
        
        while (true)
        {
            Define.SceneState sceneState = Managers.Scene.CurrentScene.SceneState;
            if (CurrentHp > 0 && sceneState != Define.SceneState.Intro && sceneState != Define.SceneState.Ending)
            {
                CurrentHp -= _hpDecayAmount;
                if (CurrentHp < 0) CurrentHp = 0;

                HpCountAction?.Invoke(CurrentHp);
            }
            yield return wait;
        }
    }

    public IEnumerator InvincibleProcess(bool blinkToggle, float invincibleTime)
    {
        IsInvincible = true;

        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null && blinkToggle) pc.StartBlinkEffect(invincibleTime);

        yield return new WaitForSeconds(invincibleTime);

        IsInvincible = false;
    }
}
//..