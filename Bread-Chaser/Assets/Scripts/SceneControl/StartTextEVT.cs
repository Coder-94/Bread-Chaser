using UnityEngine;

public class StartTextEVT : MonoBehaviour
{
   public void SoundMaker(string name)
   {
        Managers.Sound.Play($"Sounds/SE/{name}");
    }

    public void ClosePopUp()
    {
        Managers.Scene.CurrentScene.SetSceneState(Define.SceneState.DefaultPlay);
        GetComponentInParent<StartTxT>().ClosePopUpUI();
    }
}
