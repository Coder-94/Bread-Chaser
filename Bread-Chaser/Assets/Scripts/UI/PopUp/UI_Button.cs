using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Button : UIPopUp
{
    #region Enums

    enum Texts
    {
        ButtonText,
        PanelText,
        TestText
    }

    enum Buttons
    {
        Button
    }

    enum GameObjects
    {

    }

    enum Images
    {
        Mika
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