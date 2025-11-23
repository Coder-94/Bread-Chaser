using UnityEngine;

public class HpSkillController : BaseSkillController
{
    [Header("Expansion Settings")]
    [SerializeField] private float _maxRadius = 6.0f;
    [SerializeField] private float _expandSpeed = 10.0f;

    private SphereCollider  _myCollider;
    private float           _finalDamage = 0f;

    private void Awake()
    {
        duration = 2;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        transform.position = Managers.Game.GetPlayer().transform.position;
        Managers.Sound.Play("SE/GDHit");
    }

    private void Update()
    {
        if (_myCollider != null)
        {
            if (_myCollider.radius < _maxRadius)
            {
                _myCollider.radius += _expandSpeed * Time.deltaTime;
            }
        }
    }

    protected override void Init()
    {
        base.Init();

        
        _myCollider = GetComponent<SphereCollider>();
        if (_myCollider != null)
        {
            _myCollider.radius = 0.5f;
        }

        float shieldRatio = 0f;
        if (stat.MaxShield > 0)
        {
            shieldRatio = Mathf.Clamp01(stat.CurrentShield / stat.MaxShield);
        }

        float minDmg = stat.SkillCoefficient * 0.85f;
        float maxDmg = stat.SkillCoefficient * 1.5f;

        _finalDamage = Mathf.Lerp(minDmg, maxDmg, shieldRatio);

        UpdateVisuals(shieldRatio);

    }

    protected override float CalculateDamage()  { return _finalDamage; }

    private void UpdateVisuals(float ratio)
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps == null) return;

        var main = ps.main;
        Color baseColor = main.startColor.color;

        float alpha = 1.0f;

        if (ratio <= 0.25f) alpha = 0.3f;
        else if (ratio <= 0.5f) alpha = 0.5f;
        else if (ratio <= 0.75f) alpha = 0.8f;
        else alpha = 1.0f;


        baseColor.a = alpha;
        main.startColor = baseColor;


        ParticleSystem[] childrenPs = GetComponentsInChildren<ParticleSystem>();
        foreach (var child in childrenPs)
        {
            var childMain = child.main;
            Color childColor = childMain.startColor.color;
            childColor.a = alpha;
            childMain.startColor = childColor;
        }
    }

}
