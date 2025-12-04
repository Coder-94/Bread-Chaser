using UnityEngine;
using UnityEngine.UI;

public class LvUpQuitPopUp : UIPopUp
{

    GameObject parent = null;
    PlayerStat playerStat = null;
    enum Buttons
    {
        YesBtn,
        NoBtn
    }

    public override void Init()
    {
        base.Init();

        playerStat = Managers.Game.GetPlayer().GetComponent<PlayerStat>();

        Bind<Button>(typeof(Buttons));
        Button yesBtn = GetButton((int)Buttons.YesBtn);
        yesBtn.onClick.AddListener(LvUpStopYes);

        Button noBtn = GetButton((int)Buttons.NoBtn);
        noBtn.onClick.AddListener(LvUpStopNo);
    }

    public void ParrentInit(GameObject go) { parent = go; }

    void LvUpStopYes()
    {
        BtnSound();
        GameObject player = Managers.Game.GetPlayer();
        PlayerController plCon = player.GetComponent<PlayerController>();
        
        Managers.Game.SetGameState(Define.GameState.Play);
        if (plCon.BrakeForEnd)
            Managers.Scene.CurrentScene.SetSceneState(Define.SceneState.Ending);
        else
        {
            if (Managers.Game.IsBossBattleStarted)
                Managers.Scene.CurrentScene.SetSceneState(Define.SceneState.BossBattle);
            else
                Managers.Scene.CurrentScene.SetSceneState(Define.SceneState.DefaultPlay);
        }

        Managers.UI.ClosePopUpUI();
        if (parent != null)
            Managers.UI.ClosePopUpUI(parent.GetComponent<LevelUp>());

        Managers.Game.TryOpenLevelUpPopup();
    }

    void LvUpStopNo()
    {
        BtnSound();

        Managers.UI.ClosePopUpUI();

        //Managers.Game.TryOpenLevelUpPopup();
    }
}
