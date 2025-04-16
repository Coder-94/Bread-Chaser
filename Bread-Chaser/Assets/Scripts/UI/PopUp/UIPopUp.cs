using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopUp : UIBase
{
    public override void Init()
    {
        Managers.UI.SetCanvas(gameObject, true);
    }

    public virtual void ClosePopUpUI()
    {
        Managers.UI.ClosePopUpUI(this);
    }
}
