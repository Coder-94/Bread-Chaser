using System;
using System.Collections;
using UnityEngine;

public abstract class BaseMobSkill : MonoBehaviour
{
    protected GameObject warningObject;
    protected GameObject skillObject;
    protected Action _onSkillEnd;
    public GameObject parent;

    public virtual void Init(Action onSkillEnd, GameObject parentMob)
    {
        _onSkillEnd = onSkillEnd;
        parent = parentMob;

        if (warningObject == null)
            warningObject = Util.FindChild(gameObject, "Warning");

        if (warningObject != null)
        {
            Warning warnScript = warningObject.GetComponent<Warning>();

            if (warnScript != null)
            {
                warnScript.WarnEnded -= OnWarningEnded;
                warnScript.WarnEnded += OnWarningEnded;

                warnScript.WarningInit();
            }
        }
    }

    protected void OnWarningEnded()
    {
        if (warningObject != null)
        {
            Warning warnScript = warningObject.GetComponent<Warning>();
            if (warnScript != null)
                warnScript.WarnEnded -= OnWarningEnded;
        }

        StartCoroutine(SkillSequence());
    }

    protected void SelfDestroy()
    {
        _onSkillEnd?.Invoke();

        Destroy(gameObject);
    }

    protected abstract IEnumerator SkillSequence();
}
