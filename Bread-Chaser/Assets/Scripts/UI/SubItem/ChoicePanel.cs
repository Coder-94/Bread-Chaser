using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChoicePanel : UIBase, IPointerClickHandler
{
    enum Texts
    {
        Text
    }

    int     _chosenStat = 999;
    string  _statTxt = null;

    public override void Init()
    {
        Bind<TMP_Text>(typeof(Texts));
        ReRoll();
    }

    public void ReRoll() 
    {
        Debug.Log("리롤작동");
        _chosenStat = UnityEngine.Random.Range(1, 5);
        switch (_chosenStat)
        {
            case (int)Define.IncreaseAbleStat.Atk:
                _statTxt = "공격력";
                break;
            case (int)Define.IncreaseAbleStat.MoveSpd:
                _statTxt = "이동속도";
                break;
            case (int)Define.IncreaseAbleStat.Hp:
                _statTxt = "체력";
                break;
            case (int)Define.IncreaseAbleStat.SkillDMG:
                _statTxt = "스킬 데미지";
                break;
        }
        GetText((int)Texts.Text).text = $"{_statTxt} 증가";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(_chosenStat != 999)
        {
            BtnSound();
            GameObject player = Managers.Game.GetPlayer();
            PlayerController plCon = player.GetComponent<PlayerController>();
            PlayerStat plStat = player.GetComponent<PlayerStat>();

            plStat.SetStat((Define.IncreaseAbleStat)_chosenStat);
            
            Managers.Game.SetGameState(Define.GameState.Play);
            if (plCon.BrakeForEnd)
                Managers.Scene.CurrentScene.SetSceneState(Define.SceneState.Ending);
            else
            {
                if (Managers.Game.IsBossBattleStarted)
                    Managers.Scene.CurrentScene.SetSceneState(Define.SceneState.BossBattle);
                else
                    Managers.Scene.CurrentScene.SetSceneState(Define.SceneState.DefaultPlay);
            }
                


            Managers.UI.ClosePopUpUI(GetComponentInParent<LevelUp>());

            Managers.Game.TryOpenLevelUpPopup();
        }
    }
}
