using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillBtn : UIScene
{
    enum Buttons
    {
        Button
    }

    enum Images
    {
        SkillIcon,
        CoolDown
    }

    PlayerController _ctrl;
    PlayerStat _stat;
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));

        _ctrl = Managers.Game.GetPlayer().GetComponent<PlayerController>();
        _stat = Managers.Game.GetPlayer().GetComponent<PlayerStat>();
        if (_stat.LastEvolved)
        {
            SkillInit(_stat.EvolvedType);
        }
    }

    public void SkillInit(Define.IncreaseAbleStat type)
    {
        if (type == Define.IncreaseAbleStat.Default)
            return;

        Image image = GetImage((int)Images.SkillIcon);
        Button btn = GetButton((int)Buttons.Button);
        

        image.sprite = Managers.Resource.Load<Sprite>($"Textures/UI/{type}");
        RectTransform rect = image.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;

        btn.onClick.AddListener(() => _ctrl.SkillOpen());
    }

    private void Update()
    {
        Image coolTxT = GetImage((int)Images.CoolDown);
        RectTransform rect = coolTxT.GetComponent<RectTransform>();

        if (_ctrl.IsSkillCool)
        {
            rect.anchoredPosition = Vector2.zero;
        }
        else
        {
            rect.anchoredPosition = new Vector2(0f, -600f);
        }
    }

    //.
}
