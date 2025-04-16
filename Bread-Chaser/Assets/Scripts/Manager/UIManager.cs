using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    #region variables

    int _order = 10;

    Stack<UIPopUp>  _popUpStack = new Stack<UIPopUp>();
    UIScene         _sceneUI = null;

    #endregion

    #region Default Settings

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = (_order);
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }

    }

    #endregion

    #region HUD
    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UIBase
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}");
        if (parent != null)
            go.transform.SetParent(parent);

        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return Util.GetOrAddComponent<T>(go);
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UIBase
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}");
        if (parent != null)
            go.transform.SetParent(parent);

        return Util.GetOrAddComponent<T>(go);
    }

    public T ShowSceneUI<T>(string name = null) where T : UIScene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform);

        return sceneUI;
    }

    #endregion

    #region PopUp

    public T ShowPopUpUI<T>(string name = null) where T : UIPopUp
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/PopUp/{name}");
        T popUp = Util.GetOrAddComponent<T>(go);
        _popUpStack.Push(popUp);

        go.transform.SetParent(Root.transform);

        return popUp;
    }

    public void ClosePopUpUI(UIPopUp popUp)
    {
        if (_popUpStack.Count == 0)
            return;

        if(_popUpStack.Peek() != popUp)
        {
            Debug.Log("Close PopUp Failed");
            return;
        }

        ClosePopUpUI();
    }

    public void ClosePopUpUI()
    {
        if (_popUpStack.Count == 0)
            return;

        UIPopUp popUp = _popUpStack.Pop();
        Managers.Resource.Destroy(popUp.gameObject);
        popUp = null;

        _order--;
    }

    public void ClosePopUpUIAll()
    {
        while (_popUpStack.Count > 0)
            ClosePopUpUI();
    }

    #endregion

    public void Clear()
    {
        ClosePopUpUIAll();
        _sceneUI = null;
    }
}
