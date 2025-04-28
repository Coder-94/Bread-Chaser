using System.Collections;
using UnityEngine;

public class NormalMobController : BaseMobController
{
    public float targetValue = 1.0f;
    public float duration = 1.0f;

    private SkinnedMeshRenderer smr;
    private Material targetMaterial;
    private string floatName = "_DissolveHeight";
    private Coroutine lerpCoroutine;

    protected void Update()
    {
        OnUpdate(5.0f);
    }

    protected override void Init()
    {
        base.Init();

        smr = GetComponentInChildren<SkinnedMeshRenderer>();

        if (smr != null && smr.materials.Length > 1)
            targetMaterial = smr.materials[1];

        StartLerp();
    }


    void StartLerp()
    {
        if (lerpCoroutine != null)
            StopCoroutine(lerpCoroutine);

        lerpCoroutine = StartCoroutine(LerpFloat());
    }

    IEnumerator LerpFloat()
    {
        float elapsedTime = 0f;
        float startValue = targetMaterial.GetFloat(floatName);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            float newValue = Mathf.Lerp(startValue, targetValue, t);
            targetMaterial.SetFloat(floatName, newValue);

            yield return null;
        }

        targetMaterial.SetFloat(floatName, targetValue);
    }

    protected override void Clear()
    {
        //throw new System.NotImplementedException();
    }
}
