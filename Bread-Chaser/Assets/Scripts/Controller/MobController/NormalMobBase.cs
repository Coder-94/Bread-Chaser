using Data;
using System;
using System.Collections;
using UnityEngine;

public abstract class NormalMobBase : BaseMobController
{
    protected enum AnimParameters
    {
        IsDead,
        IsStunned,
        IsCasting,
        TriggerAtk,
        TriggerSpAtk,
        TriggerEncountLocalAtk,
        TriggerLocalAtk
    }

    #region variables
    
    protected Define.NormalMobStatus    myState = Define.NormalMobStatus.Idle;

    protected bool                      _spAtkToggle = false;

    private bool                        _isInitialized = false;
    private float                       _targetValue = 0;
    private float                       _duration = 0.5f;
    private SkinnedMeshRenderer         _smr;
    private Material                    _targetMaterial;
    private Coroutine                   _lerpCoroutine;

    protected NormalMobStat             mobStat;

    protected int[]                     _hashedParams;
    protected Animator                  _anim;
    private float                       _attackCooldown = 0f;

    protected int                       _spawnedRoomNum = 999;
    #endregion

    #region CurrentState
    public Define.NormalMobStatus CurrentState
    {
        get { return myState; }
        set
        {
            myState = value;

            switch (myState)
            {
                case Define.NormalMobStatus.Idle:
                    if(_isInitialized)
                        _anim.CrossFade("Idle", 0.2f);
                    break;
                case Define.NormalMobStatus.LocalAttacking:
                    _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerEncountLocalAtk]);
                    break;
                case Define.NormalMobStatus.Attacking:
                    _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerAtk]);
                    break;
                case Define.NormalMobStatus.Death:
                    Clear();
                    break;
            }
        }
    }
    #endregion

    #region Unity Scripts
    protected void OnEnable()
    {
        if (_isInitialized)
        {
            StartLerp();
            CurrentState = Define.NormalMobStatus.Spawn;
        }
    }

    protected void Update()
    {
        RotFixer(player);
        StateChecker();
    }

    private void LateUpdate()
    {
        PosFixer();
    }
    #endregion

    #region Initialize
    protected override void Init()
    {
        base.Init();

        int animParamLength = System.Enum.GetValues(typeof(AnimParameters)).Length;
        _hashedParams = new int[animParamLength];
        for (int i = 0; i < animParamLength; i++)
        {
            AnimParameters param    = (AnimParameters)i;
            string key              = param.ToString();
            _hashedParams[i]        = Animator.StringToHash(key);
        }

        mobStat     = gameObject.GetOrAddComponent<NormalMobStat>();
        _smr        = GetComponentInChildren<SkinnedMeshRenderer>();
        _anim       = GetComponent<Animator>();
        if (_smr != null && _smr.materials.Length > 1)
            _targetMaterial = _smr.materials[1];

        CurrentState = Define.NormalMobStatus.Spawn;

        _isInitialized = true;
        StartLerp();
    }
    #endregion

    #region mob Pattern

    protected void StateChecker()
    {
        if (mobStat.Hp <= 0)
            CurrentState = Define.NormalMobStatus.Death;


        switch (CurrentState)
        {
            case Define.NormalMobStatus.Idle:
                Idle();
                break;
            case Define.NormalMobStatus.Attacking:
                Attack();
                break;
            case Define.NormalMobStatus.LocalAttacking:
                LocalAtk();
                break;
            case Define.NormalMobStatus.SpecialAttacking:
                SpecialAtk();
                break;
        }
    }

    //Idle ============================================================================================================
    protected void Idle()
    {
        if (ImTargeted && player.GetComponent<PlayerController>().TargetNotDead)
        {
            _attackCooldown = 0f;
            CurrentState = Define.NormalMobStatus.LocalAttacking;
        }
        else if (!ImTargeted && player.GetComponent<PlayerController>().TargetNotDead)
            return;

            _attackCooldown += Time.deltaTime;

        if (_attackCooldown >= mobStat.AtkSpeed)
        {
            _attackCooldown = 0f;
            int random = 0;

            if (!_spAtkToggle)
                random = UnityEngine.Random.Range(0, 2);

            if (random == 0)
                CurrentState = Define.NormalMobStatus.Attacking;
            else
                CurrentState = Define.NormalMobStatus.SpecialAttacking;
        }
    }

    //Atk ============================================================================================================
    protected override void Attack()
    {
        if (ImTargeted && player.GetComponent<PlayerController>().TargetNotDead)
        {
            _attackCooldown = 0;
            CurrentState = Define.NormalMobStatus.LocalAttacking;
        }
        else if (!ImTargeted && player.GetComponent<PlayerController>().TargetNotDead)
            CurrentState = Define.NormalMobStatus.Idle;
    }

    //LocalAtk ============================================================================================================
    protected override void LocalAtk()
    {
        if (!ImTargeted)
            CurrentState = Define.NormalMobStatus.Idle;

        _attackCooldown += Time.deltaTime;

        float boomCount = 3f;

        if(_attackCooldown >= boomCount)
        {
            _attackCooldown = 0f;
            _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerLocalAtk]);
        }
    }

    #endregion

    #region Spawn Effect

    //Lerp ============================================================================================================
    void StartLerp()
    {
        if (_lerpCoroutine != null)
            StopCoroutine(_lerpCoroutine);

        _lerpCoroutine = StartCoroutine(LerpFloat());
    }

    IEnumerator LerpFloat()
    {
        string floatName =      "_DissolveHeight";
        float elapsedTime =         0f;
        float startValue =      _targetMaterial.GetFloat(floatName);

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / _duration);

            float newValue = Mathf.Lerp(startValue, _targetValue, t);
            _targetMaterial.SetFloat(floatName, newValue);

            yield return null;
        }

        _targetMaterial.SetFloat(floatName, _targetValue);
    }

    //RoomNumInit ============================================================================================================
    public void SpawnedRoomNumSet(int loofedRoomNum)
    {
        _spawnedRoomNum = loofedRoomNum;
    }
    #endregion

    #region Clear

    //Clear ============================================================================================================
    protected override void Clear()
    {
        if (_spawnedRoomNum == 999)
        {
            Debug.Log("Spawned Room Number Not Initialized!");
            return;
        }

        //outer variables clear
        mobStat.Clear();
        Managers.Scene.CurrentScene.spawnedMobChecker[_spawnedRoomNum].isSpawned = false;
        Managers.Scene.CurrentScene.MobCountController(false);

        //inner variables clear
        _targetMaterial.SetFloat("_DissolveHeight", 1);
        _targetValue = 0;
        _duration = 0.5f;
        _spawnedRoomNum = 999;
        ImTargeted = false;

        //destroy self
        Managers.Resource.Destroy(gameObject);
    }
    #endregion

    protected override void PosFixer()
    {
        if(CurrentState == Define.NormalMobStatus.Spawn)
        {
            Vector3 newPosition = transform.position;
            Vector3 playerPos = player.GetComponent<PlayerController>().OriginPos;

            if (playerPos == new Vector3(9999, 9999, 9999))
                playerPos = player.transform.position;

            newPosition.x = playerPos.x + initPos.x;

            transform.position = newPosition;

            return;
        }
        base.PosFixer();
        
    }
    protected abstract void SpecialAtk();
}
