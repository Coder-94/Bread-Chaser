using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUp : UIPopUp
{
    enum Buttons
    {
        QuitBtn,
        ReRollBtn
    }

    enum Texts 
    { 
        ReRollText
    }

    int         _choosingNum = 0;
    int         _reRollChance = 0;
    Vector2     firstPos = Vector2.zero;

    public override void Init()
    {
        base.Init();
        Data.GatchaCheck gatchaCheck = Managers.Data.GatchaDict["gatcha"];

        _choosingNum = gatchaCheck.choosingNum;
        _reRollChance = gatchaCheck.reRollChance;

        Transform root = Util.FindChild(gameObject, "Grid").transform;

        for (int i=0; i< _choosingNum; i++) 
        {
            ChoicePanel go = Managers.UI.MakeSubItem<ChoicePanel>(root);
            go.transform.SetAsFirstSibling();
        }
        _reRollChance = 1;

        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));

        Button quitBtn = GetButton((int)Buttons.QuitBtn);
        quitBtn.onClick.AddListener(Quit);

        Button rerollBtn = GetButton((int)Buttons.ReRollBtn);
        rerollBtn.onClick.AddListener(ReRoll);

        TMP_Text rerollText = GetText((int)Texts.ReRollText);
        rerollText.text = $"Reroll Count : {_reRollChance}";

    }

    void ReRoll()
    {
        BtnSound();
        if(_reRollChance > 0)
        {
            if(_choosingNum == 1)
            {
                Util.FindChild<ChoicePanel>(gameObject, "ChoicePanel", true).ReRoll();
            }
            else
            {
                List<ChoicePanel> foundComponents = transform.Cast<Transform>()
                                                        .Select(child => child.GetComponent<ChoicePanel>())
                                                        .Where(component => component != null)
                                                        .ToList();

                foreach (ChoicePanel controller in foundComponents)
                {
                    controller.ReRoll();
                }
            }
            
            _reRollChance--;
            TMP_Text rerollText = GetText((int)Texts.ReRollText);
            rerollText.text = $"Reroll Count : {_reRollChance}";
        }
        
    }

    void Quit() 
    { 
        LvUpQuitPopUp quitPopUp = Managers.UI.ShowPopUpUI<LvUpQuitPopUp>("LvUpQuitPopUp", 1);
        quitPopUp.ParrentInit(this.gameObject);
    }
}
