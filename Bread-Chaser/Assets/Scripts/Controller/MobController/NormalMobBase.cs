using Data;
using System;
using System.Collections;
using UnityEngine;

public class NormalMobBase : BaseMobController
{
    protected enum AnimParameters
    {
        IsDead,
        IsStunned,
        IsCasting,
        TriggerAtk,
        TriggerSpAtk
    }

    #region variables
    protected Define.NormalMobStatus    myState = Define.NormalMobStatus.Idle;

    private bool                        _isInitialized = false;
    private float                       _targetValue = 0;
    private float                       _duration = 0.5f;
    private SkinnedMeshRenderer         _smr;
    private Material                    _targetMaterial;
    private Coroutine                   _lerpCoroutine;

    protected NormalMobStat             mobStat;
    private Vector3                     _spawnPos;

    protected int[]                     _hashedParams;
    protected Animator                  _anim;
    private float                       _attackCooldown = 0f;

    protected int                       _spawnedRoomNum = 999;
    #endregion

    #region Unity Scripts
    protected void OnEnable()
    {
        _spawnPos = gameObject.transform.position;

        if (_isInitialized)
            StartLerp();
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

        _isInitialized = true;
        StartLerp();
    }
    #endregion

    #region Spawn Effect
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
    #endregion


    protected override void Attack()
    {
        _attackCooldown += Time.deltaTime;

        if (_attackCooldown >= mobStat.AtkSpeed)
        {
            _attackCooldown = 0f;
            _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerAtk]);
        }
    }

    protected override void LocalAtk()
    {

    }

    protected override void Death()
    {
        
    }


    public void SpawnedRoomNumSet(int loofedRoomNum)
    {
        _spawnedRoomNum = loofedRoomNum;
    }

    #region Clear
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

        //destroy self
        Managers.Resource.Destroy(gameObject);
    }
    #endregion
}
