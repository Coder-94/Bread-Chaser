using UnityEngine;

public class MpSkillController : BaseSkillController
{
    private float           _moveSpeed = 20f;
    private BoxCollider     _col;
    private Vector3         _initialCenter;


    private void Awake()
    {
        duration = 2f;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        transform.position = Managers.Game.GetPlayer().transform.position;

        if (_col != null)
        {
            _col.center = _initialCenter;
        }

        Managers.Sound.Play("SE/Slash");
    }

    private void Update()
    {
        if (_col != null)
        {
            Vector3 newCenter = _col.center;
            newCenter.x += _moveSpeed * Time.deltaTime;
            _col.center = newCenter;
        }
    }

    protected override void Init()
    {
        base.Init();
       
        _col = GetComponent<BoxCollider>();
        _initialCenter = _col.center;
    }

    protected override float CalculateDamage() { return stat.SkillCoefficient * 1.8f; }

}

