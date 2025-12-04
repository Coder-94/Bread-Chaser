using UnityEngine;
using UnityEngine.UI;

public class GameQuitPopUp : UIPopUp
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
        yesBtn.onClick.AddListener(QuitYes); 
        
        Button noBtn = GetButton((int)Buttons.NoBtn);
        noBtn.onClick.AddListener(QuitNo);
    }

    void QuitYes()
    {
        BtnSound();
        Application.Quit();
    }

    void QuitNo()
    {
        BtnSound();
        ClosePopUpUI();
    }
}
