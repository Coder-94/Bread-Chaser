using TMPro;
using UnityEngine;

public class StatPanel : UIBase
{
    enum Texts
    {
        Text
    }

    public override void Init()
    {
        Bind<TMP_Text>(typeof(Texts));
        TMP_Text text = GetText((int)Texts.Text);

        PlayerStat stat = Managers.Game.GetPlayer().GetComponent<PlayerStat>();
        text.text = $"Power : {stat.Atk}\nHp : {stat.Hp}\nRunningSpeed : {stat.MoveSpeed}\nSkill Damage : {stat.SkillCoefficient}";
    }
}
