using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPopUp : UIPopUp
{
    float duration = 5;

    enum Texts
    {
        Score,
        Timer
    }

    private void Update()
    {
        GetText((int)Texts.Timer).text = $"Back to title in {duration} Seconds..";
    }

    public override void Init()
    {
        Bind<TMP_Text>(typeof(Texts));

        GetText((int)Texts.Score).text = $"Last Score : {Managers.Game.ScorePoint}";

        StartCoroutine(SceneMove());
    }

    IEnumerator SceneMove()
    {
        while (duration > 0)
        {
            yield return new WaitForSeconds(1f);
            duration -= 1f;
            yield return null;
        }

        Managers.Game.ClearMob();
        Managers.Scene.LoadScene(Define.Scene.Title);
    }

}
