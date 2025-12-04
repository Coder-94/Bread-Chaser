using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeOutPopUp : UIPopUp
{
    public float fadeDuration = 1.0f;
    CanvasGroup  fader;


    enum Images
    {
        Fader
    }

    public override void Init()
    {
        base.Init();
        Debug.Log("Fade Spawned");
        Canvas canvas = Util.GetOrAddComponent<Canvas>(gameObject);
        canvas.sortingOrder = 999;
        Bind<Image>(typeof(Images));
        fader = GetImage((int)Images.Fader).GetComponent<CanvasGroup>();

        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fader.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        fader.alpha = 0f;
        ClosePopUpUI();

    }
}
