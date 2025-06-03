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
    private float                       _targetValue = 0;
    private float                       _duration = 0.5f;
    private SkinnedMeshRenderer         _smr;
    private Material                    _targetMaterial;
    private Coroutine                   _lerpCoroutine;

    private NormalMobStat               mobStat;
    private Vector3                     _spawnPos;

    protected int[]                     _hashedParams;
    protected Animator                  _anim;
    private float                       _attackCooldown = 0f;
    #endregion

    #region Unity Scripts
    protected void OnEnable()
    {
        _spawnPos = gameObject.transform.position;
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

    protected void Idle()
    {
        _attackCooldown += Time.deltaTime;

        if (_attackCooldown >= mobStat.AtkSpeed)
        {
            _attackCooldown = 0f;
            _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerAtk]);
        }
    }

    #region Clear
    protected override void Clear()
    {
        for(int i=0; i<Managers.Scene.CurrentScene.spawnedMobChecker.Length; i++)
        {
            if (_spawnPos == Managers.Scene.CurrentScene.spawnedMobChecker[i].spawnedPos)
            {
                Managers.Scene.CurrentScene.spawnedMobChecker[i].isEnable = false;
                Managers.Scene.CurrentScene.monsterCount--;
                break;
            }
        }
            
    }
    #endregion
}
