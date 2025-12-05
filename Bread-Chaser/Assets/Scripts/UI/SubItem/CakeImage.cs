using UnityEngine;

public class CakeImage : UIBase
{
    public override void Init()
    {

    }

    public void SoundMaker()
    {
        Managers.Sound.Play($"Sounds/SE/End");
    }
}
