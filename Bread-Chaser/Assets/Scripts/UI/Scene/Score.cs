using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Score : UIScene
{
    TMP_Text    _text;
    

    enum Texts 
    { 
        ScorePoint
    }

    private void Update()
    {
        ScoreShower();
    }

    void ScoreShower()
    {
        _text.text = $"Score: {Managers.Game.ShowScore()}";
    }

    public override void Init()
    {
        base.Init();
        Bind<TMP_Text>(typeof(Texts));

        _text = GetText((int)Texts.ScorePoint);

        _text.text = $"Score: {Managers.Game.ShowScore()}";
    }


}
