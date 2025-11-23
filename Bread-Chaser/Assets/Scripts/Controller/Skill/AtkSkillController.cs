using UnityEngine;

public class AtkSkillController : BaseSkillController
{
    float _defaultBuffRange = 0.25f;
 
    float _originAtk;
    private void Awake()
    {
        duration = 10f;
    }

    protected override void Init()
    {
        base.Init();

        _originAtk = stat.Atk;

        float buffPercent = _defaultBuffRange + Mathf.Log10(stat.SkillCoefficient + 1f) * 0.4f;

        float buffResult = _originAtk * (1f + buffPercent);

        stat.BuffStat(Define.IncreaseAbleStat.Atk, buffResult, true);

        Debug.Log($"Atk {_originAtk}, CurrentAtk {stat.Atk}");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        gameObject.transform.parent = Managers.Game.GetPlayer().transform;
        Managers.Sound.Play($"SE/Buff");
    }

    public void SelfDestroy()
    {
        if (stat != null)
        {
            stat.BuffStat(Define.IncreaseAbleStat.Atk, _originAtk, false);
        }
        Managers.Resource.Destroy(gameObject);
    }

    protected override float CalculateDamage(){ return 0; }

}
