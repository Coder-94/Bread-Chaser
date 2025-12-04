using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : UIScene
{
    Slider _bgmSlider;
    Slider _seSlider;

    Button _quitBtn;
    Button _seBtn;
    Button _bgmBtn;
    Button _openBtn;
    Button _optionBtn;
    Button _exitBtn;

    Sprite[] _previousBtnImages = new Sprite[System.Enum.GetValues(typeof(Sliders)).Length + 1];

    Vector3 optionPanelSize = new Vector3(0.78f, 0.35f, 0.57f);

    enum GameObjects
    {
        OptionPanel
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
        OpenBtn,
        OptionBtn,
        ExitBtn
    }

    /*
    private void OnEnable()
    {
        for (int btn = 0; btn < System.Enum.GetValues(typeof(Sliders)).Length; btn++)
        {
            Slider slider = GetSlider(btn);
            Image image = GetButton(btn).image;

            if (_previousBtnImages[btn] != null)
                image.sprite = _previousBtnImages[btn];
            _previousBtnImages[btn] = null;

            slider.value = Managers.Sound.LoadCurrentVolume((Define.Sound)btn);
        }
    }
    */

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
        _seBtn = GetButton((int)Buttons.SEBtn);
        _bgmBtn = GetButton((int)Buttons.BGMBtn);
        _openBtn = GetButton((int)Buttons.OpenBtn);
        _optionBtn = GetButton((int)Buttons.OptionBtn);
        _exitBtn = GetButton((int)Buttons.ExitBtn);

        _quitBtn.onClick.AddListener(Quit);
        _seBtn.onClick.AddListener(() => BtnVolumeOnOff((int)Buttons.SEBtn));
        _bgmBtn.onClick.AddListener(() => BtnVolumeOnOff((int)Buttons.BGMBtn)); 
        _openBtn.onClick.AddListener(Open);
        _optionBtn.onClick.AddListener(Option);
        _exitBtn.onClick.AddListener(Exit);
        ControlPanel(false);
    }

    void Exit() { Application.Quit(); }
    void Open() { BtnSound(); StartCoroutine(ScenePassCoroution()); }

    IEnumerator ScenePassCoroution() { yield return new WaitForSeconds(0.3f); Managers.Scene.LoadScene(Define.Scene.City); }
    void Option() { BtnSound(); ControlPanel(true); }

    void Quit()
    {
        BtnSound();

        for (int btn = 0; btn < System.Enum.GetValues(typeof(Sliders)).Length + 1; btn++)
        {
            Image image = GetButton(btn).image;
            _previousBtnImages[btn] = image.sprite;
        }
        Managers.Sound.SaveCurrentVolume();
        ControlPanel(false);
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
        else if (volumeState.sprite.name == "music off")
        {
            volumeState.sprite = Managers.Resource.Load<Sprite>("Textures/UI/music on");
            Managers.Sound.OnOffVolume(slider, (Define.Sound)btn, false);
        }

        volumeState.SetNativeSize();
    }

    void ControlPanel(bool On)
    {
        RectTransform rect = GetObject((int)GameObjects.OptionPanel).GetComponent<RectTransform>();

        if(On)
            rect.localScale = optionPanelSize;
        else
            rect.localScale = Vector3.zero;
    }
}
