using UnityEngine;
using UnityEngine.UI;

public class LvUpQuitPopUp : UIPopUp
{
    enum Buttons
    {
        YesBtn,
        NoBtn
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Button yesBtn = GetButton((int)Buttons.YesBtn);
        yesBtn.onClick.AddListener(LvUpStopYes);

        Button noBtn = GetButton((int)Buttons.NoBtn);
        noBtn.onClick.AddListener(LvUpStopNo);
    }

    void LvUpStopYes()
    {
        BtnSound();
        Managers.UI.ClosePopUpUIAll();
        Managers.Game.SetGameState(Define.GameState.Play);
        Managers.Scene.CurrentScene.SetSceneState(Define.SceneState.DefaultPlay);
    }

    void LvUpStopNo()
    {
        BtnSound();
        Managers.UI.ClosePopUpUI();
    }
}
