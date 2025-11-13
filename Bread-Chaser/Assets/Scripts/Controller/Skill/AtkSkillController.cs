using UnityEngine;

public class AtkSkillController : MonoBehaviour
{
    float _duration = 10f;
    float _timer;

    float _defaultBuffRange = 0.25f;
 
    float _originAtk;

    public bool buff;

    void OnEnable()
    {
        _timer = _duration;
    }

    void Update()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0)
            {
                SelfDestroy();
            }
        }
    }

    void Start()
    {
        Managers.Sound.Play($"SE/Buff");
        gameObject.transform.parent = Managers.Game.GetPlayer().transform;
        buff = true;
        PlayerStat stat = GetComponentInParent<PlayerStat>();

        _originAtk = stat.Atk;

        float buffPercent = _defaultBuffRange + Mathf.Log10(stat.SkillCoefficient + 1f) * 0.4f;

        float buffResult = _originAtk * (1f + buffPercent);

        stat.BuffStat(Define.IncreaseAbleStat.Atk, buffResult);

        Debug.Log($"Atk {_originAtk}, CurrentAtk {stat.Atk}");
    }

    public void SelfDestroy()
    {
        PlayerStat stat = GetComponentInParent<PlayerStat>();
        stat.BuffStat(Define.IncreaseAbleStat.Atk, _originAtk);
        buff = false;
        Managers.Resource.Destroy(gameObject);
    }
}
