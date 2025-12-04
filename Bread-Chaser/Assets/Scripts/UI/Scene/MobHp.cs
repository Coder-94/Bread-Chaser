using UnityEngine;

public class MobHp : UIScene
{
    enum Sliders
    {
        HpSlider
    }

    GameObject _boss;

    public override void Init()
    {
        base.Init();

        _boss = GameObject.FindGameObjectWithTag("Boss");
        Stat bossHp = _boss.GetComponent<Stat>();

        Bind<UnityEngine.UI.Slider>(typeof(Sliders));

        bossHp.HpCountAction -= HPBarControl;
        bossHp.HpCountAction += HPBarControl;
    }

    void HPBarControl(float currentHp)
    {
        GetSlider((int)Sliders.HpSlider).value = currentHp / _boss.GetComponent<Stat>().Hp;
    }
}
