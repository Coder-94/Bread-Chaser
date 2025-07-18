using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_PopupObjects : UIPopUp
{
    #region Enums

    enum Texts
    {

    }

    enum Buttons
    {
        Button
    }

    enum GameObjects
    {
        LockOn,
        OnTarget,
        TargetCursor,
        OnTargetCursor
    }

    enum Images
    {
        
    }

    #endregion

    public override void Init()
    {

        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
    }

}