using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingScene : UIScene
{
    float duration = 8;
    float finalResult = 0;
    Define.Scene scene;

    enum Texts
    {
        Score,
        Cookies,
        BossKill,
        Result,
        Timer
    }

    enum Btns
    {
        PassBtn
    }

    private void Update()
    {
        if (scene != Define.Scene.Universe)
            GetText((int)Texts.Timer).text = $"Next Stage Apears in {duration} Seconds..";
            
    }
    
    public void PanelInit(float playTime, float cookieNum, float exp)
    {
        float cookieScore = 1f + Mathf.Log10(cookieNum + 1f) * 0.5f;
        scene = Managers.Scene.CurrentScene.SceneType;

        float timeBonus;
        if (playTime > 6500)
            timeBonus = 0.8f;
        else if (playTime > 6000)
            timeBonus = 1f;
        else
            timeBonus = 1.3f;

        float finalBonus = exp * cookieScore * timeBonus;

        Bind<TMP_Text>(typeof(Texts));
        Bind<Button>(typeof(Btns));

        GetButton((int)Btns.PassBtn).onClick.AddListener(PassBtn);
        GetText((int)Texts.Score).text = $"Score : {playTime} Sec";
        GetText((int)Texts.Cookies).text = $"Ate Cookies : {cookieNum}";
        GetText((int)Texts.BossKill).text = $"BossKillPoint : {exp}";
        GetText((int)Texts.Result).text = $"Stage Clear Point\n{finalBonus}";

        if (scene == Define.Scene.Universe)
        {
            GetButton((int)Btns.PassBtn).gameObject.SetActive(false);
            duration = 3.5f;
            LastScoreForUni(finalBonus);
        }
        else
            StartCoroutine(LastScore(finalBonus));

        StartCoroutine(SceneMove());
        
    }

    void PassBtn()
    {
        BtnSound();
        SceneCheck();
    }

    void SceneCheck()
    {
        if(scene == Define.Scene.Universe)
            Managers.Scene.LoadScene(Define.Scene.Title);
        else
            Managers.Scene.LoadScene(scene + 1);
    }

    IEnumerator SceneMove() 
    {
        if(scene == Define.Scene.Universe)
        {
            yield return new WaitForSeconds(duration);
            Managers.UI.ShowPopUpUI<CakePopUp>();
        }
        else
        {
            while (duration > 0)
            {
                yield return new WaitForSeconds(1f);
                duration -= 1f;
                yield return null;
            }

            SceneCheck();
        }
    }

    IEnumerator LastScore(float finalBonus)
    {
        yield return new WaitForSeconds(1.5f);

        Managers.Game.AddScore(finalBonus);
    }

    void LastScoreForUni(float finalBonus)
    {
        if (scene == Define.Scene.Universe)
        {
            Managers.Game.AddScore(finalBonus);
            finalResult = Managers.Game.ScorePoint;
            GetText((int)Texts.Timer).text = $"Final Score : {finalResult}";
            return;
        }
    }
}
