using System;
using System.Collections;
using UnityEngine;

public class IceLandSkill : BaseMobSkill
{
    [SerializeField] private float skillDuration = 3.0f;

    public override void Init(Action onSkillEnd, GameObject parentMob)
    {
        base.Init(onSkillEnd, parentMob);

        _onSkillEnd = onSkillEnd;
        parent = parentMob;

        if (warningObject == null)
            warningObject = Util.FindChild(gameObject, "Warning");

        if (skillObject == null)
            skillObject = Util.FindChild(gameObject, "Skill");

        if (skillObject != null)
            skillObject.SetActive(false);
    }

    protected override IEnumerator SkillSequence()
    {
        if (skillObject != null)
        {
            skillObject.SetActive(true);
            Managers.Sound.Play("SE/Whip");

            SwordBeam projectile = skillObject.GetComponent<SwordBeam>();
        }

        yield return new WaitForSeconds(skillDuration);

        SelfDestroy();
    }


}
