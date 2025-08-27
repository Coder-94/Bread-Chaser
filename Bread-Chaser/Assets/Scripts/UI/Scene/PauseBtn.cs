using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseBtn : UIScene
{
    Button _btn;

    enum Buttons 
    {
        Btn
    }

    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));

        _btn = GetButton((int)Buttons.Btn);

        _btn.onClick.AddListener(Pause);
    }

    void Pause()
    {
        if (Managers.Scene.CurrentScene.SceneState == Define.SceneState.Intro)
            return;

        Time.timeScale = 0f;
        Managers.UI.ShowPopUpUI<PausePopUp>("PausePopUp", 1);
    }
}
