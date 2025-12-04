using System;
using UnityEngine;
using UnityEngine.UI;

public class BossAtkBtn : UIPopUp
{
    enum Buttons
    {
        Button
    }

    private void Update()
    {
        if (Managers.Scene.CurrentScene.SceneState == Define.SceneState.Ending)
            ClosePopUpUI();
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.Button).onClick.AddListener(BossAtk);
    }

    void BossAtk()
    {
        GameObject pl = Managers.Game.GetPlayer();
        PlayerController plCon = pl.GetComponent<PlayerController>();

        if(plCon != null) 
        {
            BtnSound();
            plCon.BossAtkTrigger();
            Managers.UI.ClosePopUpUI(this);
        }
    }
}
