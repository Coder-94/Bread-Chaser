using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : BaseScene
{
    protected new void Awake()
    {
        Init();
    }

    protected new void Init()
    {
        SceneType = Define.Scene.Title;
        SceneName = SceneManager.GetActiveScene().name;
        SceneState = Define.SceneState.Intro;
        Managers.UI.ShowPopUpUI<FadeOutPopUp>();
    }

    protected new void Update()
    {
        
    }
}
