using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Hp : UIScene
{
    enum Sliders
    {
        HpSlider
    }

    GameObject          _player;
    public override void Init()
    {
        base.Init();

        _player = Managers.Game.GetPlayer();
        PlayerStat playerHp = _player.GetComponent<PlayerStat>();

        Bind<UnityEngine.UI.Slider>(typeof(Sliders));

        playerHp.HpCountAction -= HPBarControl;
        playerHp.HpCountAction += HPBarControl;
    }

    void HPBarControl(float currentHp)
    {
        GetSlider((int)Sliders.HpSlider).value = currentHp / _player.GetComponent<PlayerStat>().Hp;
    }

}
