using System.Collections;
using TMPro;
using UnityEngine;

public class CakePopUp : UIPopUp
{
    float duration = 8;

    enum Texts
    {
        CountText
    }
    void Start()
    {
        Bind<TMP_Text>(typeof(Texts));
        StartCoroutine(SceneMove());
    }

    private void Update()
    {
        GetText((int)Texts.CountText).text = $"Go Back To Title In\n{duration} Seconds..";
    }

    IEnumerator SceneMove()
    {
        while (duration > 0)
        {
            yield return new WaitForSeconds(1f);
            duration -= 1f;
            yield return null;
        }

        Managers.Scene.LoadScene(Define.Scene.Title);
    }
}
