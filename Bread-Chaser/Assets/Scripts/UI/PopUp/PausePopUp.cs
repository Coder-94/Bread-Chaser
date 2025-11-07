using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PausePopUp : UIPopUp
{
    Slider      _bgmSlider;
    Slider      _seSlider;

    Button      _quitBtn;
    Button      _endBtn;
    Button      _seBtn;
    Button      _bgmBtn;

    Sprite[]    _previousBtnImages = new Sprite[System.Enum.GetValues(typeof(Sliders)).Length + 1] ;

    enum GameObjects
    {
        Panel
    }

    enum Sliders
    {
        BGMSlider,
        SESlider
    }

    enum Buttons
    {
        BGMBtn,
        SEBtn,
        QuitBtn,
        EndBtn
    }

    private void OnEnable()
    {
        for(int btn=0; btn< System.Enum.GetValues(typeof(Sliders)).Length; btn++)
        {
            Slider slider = GetSlider(btn);
            Image image = GetButton(btn).image;

            if (_previousBtnImages[btn] != null)
                image.sprite = _previousBtnImages[btn];
            _previousBtnImages[btn] = null;

            slider.value = Managers.Sound.LoadCurrentVolume((Define.Sound)btn);
        }
    }

    public override void Init()
    {
        base.Init();

        for (int btn = 0; btn < _previousBtnImages.Length; btn++)
            _previousBtnImages[btn] = null;

        Bind<GameObject>(typeof(GameObjects));
        Bind<Slider>(typeof(Sliders));
        Bind<Button>(typeof(Buttons));

        _bgmSlider = GetSlider((int)Sliders.BGMSlider);
        _bgmSlider.onValueChanged.AddListener((float value) =>
        { Managers.Sound.SetVolume(value, Define.Sound.Bgm); });
        _seSlider = GetSlider((int)Sliders.SESlider);
        _seSlider.onValueChanged.AddListener((float value) =>
        { Managers.Sound.SetVolume(value, Define.Sound.Effect); });

        _quitBtn = GetButton((int)Buttons.QuitBtn);
        _endBtn = GetButton((int)Buttons.EndBtn);
        _seBtn = GetButton((int)Buttons.SEBtn);
        _bgmBtn = GetButton((int)Buttons.BGMBtn);

        _quitBtn.onClick.AddListener(Quit);
        _endBtn.onClick.AddListener(End);
        _seBtn.onClick.AddListener(() => BtnVolumeOnOff((int)Buttons.SEBtn));
        _bgmBtn.onClick.AddListener(() => BtnVolumeOnOff((int)Buttons.BGMBtn));
    }

    void Quit()
    {
        BtnSound();

        if(Managers.Scene.CurrentScene.SceneState != Define.SceneState.LevelUp)
        {
            Managers.Game.SetGameState(Define.GameState.Play);
        }

        for (int btn = 0; btn < System.Enum.GetValues(typeof(Sliders)).Length + 1; btn++)
        {
            Image image = GetButton(btn).image;
            _previousBtnImages[btn] = image.sprite;
        }
        Managers.Sound.SaveCurrentVolume();
        ClosePopUpUI();
    }

    void End()
    {
        BtnSound();
       Managers.UI.ShowPopUpUI<GameQuitPopUp>("GameQuitPopUp", 1);
    }

    void BtnVolumeOnOff(int btn)
    {
        BtnSound();
        Image volumeState = GetButton(btn).image;
        Slider slider = GetSlider(btn);

        if (volumeState.sprite.name == "music on")
        {
            volumeState.sprite = Managers.Resource.Load<Sprite>("Textures/UI/music off");
            Managers.Sound.OnOffVolume(slider, (Define.Sound)btn, true);
        }
        else if(volumeState.sprite.name == "music off")
        {
            volumeState.sprite = Managers.Resource.Load<Sprite>("Textures/UI/music on");
            Managers.Sound.OnOffVolume(slider, (Define.Sound)btn, false);
        }

        volumeState.SetNativeSize();
    }

}
