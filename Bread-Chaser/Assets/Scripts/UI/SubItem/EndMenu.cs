using System.Collections;
using TMPro;
using UnityEngine;

public class EndMenu : UIBase
{
    public override void Init()
    {

    }

    public void SoundMaker()
    {
        Managers.Sound.Play($"Sounds/SE/End");
    }
}
