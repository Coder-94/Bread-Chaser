using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Hp : UIScene
{
    GameObject          _player;
    float               _space = 130f;
    Vector2             _firstHeartPos;

    Stack<GameObject>   _heartStack = new Stack<GameObject>();

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _player.GetComponent<PlayerStat>().OnDamaged();
            Debug.Log($"Btn Down, {_player.GetComponent<PlayerStat>().CurrentHp}");
        }
        else if (Input.GetMouseButtonDown(1))
        {
            _player.GetComponent<PlayerStat>().Heal();
            Debug.Log($"Btn RightDown, {_player.GetComponent<PlayerStat>().CurrentHp}");
        }
    }

    public override void Init()
    {
        base.Init();
        _firstHeartPos = new Vector2(-650, 1073);

        _player = Managers.Scene.CurrentScene.Player;
        PlayerStat playerHp = _player.GetComponent<PlayerStat>();

        for (int i = 0; i < playerHp.CurrentHp; i++)
        {
            if (i == 0)
                HpAdd(true);
            else
                HpAdd(false);
        }

        playerHp.HpCountAction -= HPStackControl;
        playerHp.HpCountAction += HPStackControl;
    }

    void HpAdd(bool isInit = false)
    {
        //GameObject go = Managers.Resource.Instantiate("UI/Scene/Heart", gameObject.transform, 5);
        GameObject go = Managers.UI.MakeSubItem<Heart>(parent: gameObject.transform).gameObject;
        if (!isInit)
        {
            GameObject prevGO = _heartStack.Peek();
            Vector2 destPos = prevGO.GetComponent<RectTransform>().anchoredPosition;
            destPos.x += _space;

            go.GetComponent<RectTransform>().anchoredPosition = destPos;
        }
        else
        {
            RectTransform goRect = go.GetComponent<RectTransform>();
            goRect.anchoredPosition = _firstHeartPos;
        }

        _heartStack.Push(go);
    }

    void HPStackControl(string stackCheck)
    {
        if (stackCheck == "-")
        {
            GameObject go = _heartStack.Pop();
            Managers.Resource.Destroy(go);
        }
        else if (stackCheck == "+")
        {
            HpAdd();
        }
    }
}
