using Data;
using System.Collections;
using UnityEngine;

public class NormalMobController : BaseMobController
{
    #region variables
    private float                       _targetValue = 0;
    private float                       _duration = 0.5f;
    private SkinnedMeshRenderer         _smr;
    private Material                    _targetMaterial;
    private Coroutine                   _lerpCoroutine;

    private NormalMobStat               mobStat;

    private Vector3                     _spawnPos;

    private Define.NormalMobState       _state;
    private Animator                    _anim;
    #endregion

    #region Unity Scripts
    protected void OnEnable()
    {
        _spawnPos = gameObject.transform.position;
    }

    protected void Update()
    {
        StateChecker();
        PosFixer(new Vector3(gameObject.transform.position.x,
                             gameObject.transform.position.y,
                             player.transform.position.z + 5f));
        RotFixer(player);
    }
    #endregion

    #region Initialize
    protected override void Init()
    {
        base.Init();

        mobStat     = gameObject.GetOrAddComponent<NormalMobStat>();
        _state      = Define.NormalMobState.Spawn;
        _anim       = GetComponent<Animator>();

        _smr = GetComponentInChildren<SkinnedMeshRenderer>();
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

        string floatName = "_DissolveHeight";
        float elapsedTime = 0f;
        float startValue = _targetMaterial.GetFloat(floatName);

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

    protected override void StateChecker()
    {
        
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
