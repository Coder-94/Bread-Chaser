using System;
using System.Collections;
using UnityEngine;

public class ForestSkill : BaseMobSkill
{
    public override void Init(Action onSkillEnd, GameObject parentMob)
    {
        base.Init(onSkillEnd, parentMob);

        if (skillObject == null)
        {
            skillObject = Util.FindChild(gameObject, "LightningGroup");
        }
        
    }
    protected override IEnumerator SkillSequence()
    {
        if (skillObject != null)
        {
            skillObject.SetActive(true);
            ParticleSystem[] lightningChildren = skillObject.GetComponentsInChildren<ParticleSystem>(true);
            Managers.Sound.Play("SE/Lightning");
            foreach (ParticleSystem particle in lightningChildren)
            {
                if (particle != null)
                {
                    particle.gameObject.SetActive(true);
                    particle.Play();
                }
            }

            yield return new WaitForSeconds(0.2f);

            SelfDestroy();
        }
    }
    
}
