using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpdSkillController : BaseSkillController
{
   

    public float moveSpeed = 30f;

    private void Awake()
    {
        duration = 2f;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        transform.position = Managers.Game.GetPlayer().transform.position;
        
        Managers.Sound.Play("SE/Tornado");
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    protected override float CalculateDamage() { return stat.SkillCoefficient * 0.69f; }

    protected override void OnHitAdditionalEffect(Stat enemyStat)
    {
        enemyStat.OnSkillPoisoned(stat.gameObject);
    }
}
