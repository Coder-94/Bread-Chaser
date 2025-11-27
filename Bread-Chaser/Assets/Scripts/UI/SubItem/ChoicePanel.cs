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
        _chosenStat = UnityEngine.Random.Range(0, 4);
        _chosenStat = 1;
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

            player.GetComponent<PlayerStat>().SetStat((Define.IncreaseAbleStat)_chosenStat);

            Managers.UI.ClosePopUpUIAll();
            Managers.Game.SetGameState(Define.GameState.Play);
            Managers.Scene.CurrentScene.SetSceneState(Define.SceneState.DefaultPlay);
            Managers.Game.TryOpenLevelUpPopup();
        }
    }
}
