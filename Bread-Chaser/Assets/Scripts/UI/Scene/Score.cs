using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Score : UIScene
{
    public int  ScorePoint { get; private set; } = 0;
    TMP_Text    _text;
    float       _scoreCounter = 0f;
    int         _magnification = 10;

    enum Texts 
    { 
        ScorePoint
    }

    private void Update()
    {
        if(Managers.Scene.CurrentScene.SceneState != Define.SceneState.Intro)
            ScoreChecker();
    }

    void ScoreChecker()
    {
        //_scoreCounter += 1f * Time.deltaTime * //플레이어꺼무브스피드변경;  moveSpd per sec
        ScorePoint = Mathf.FloorToInt(_scoreCounter);
        _text.text = $"Score: {ScorePoint}";
    }

    public override void Init()
    {
        base.Init();
        Bind<TMP_Text>(typeof(Texts));

        _text = GetText((int)Texts.ScorePoint);

        _text.text = $"Score: {ScorePoint}";
    }


}
