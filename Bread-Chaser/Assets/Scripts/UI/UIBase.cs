using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UIBase : MonoBehaviour
{
    #region varaibles & properties & abstract

    Dictionary<Type, UnityEngine.Object[]>          _objects = new Dictionary<Type, UnityEngine.Object[]>();

    protected   GameObject  GetObject(int index)    { return Get<GameObject>(index); }
    protected   TMP_Text    GetText(int index)      { return Get<TMP_Text>(index); }
    protected   Button      GetButton(int index)    { return Get<Button>(index); }
    protected   Slider      GetSlider(int index)    { return Get<Slider>(index); }
    protected   Image       GetImage(int index)     { return Get<Image>(index); }

    public abstract void Init();

    #endregion

    private void Awake()
    {
        Init();
    }

    protected void BtnSound() { Managers.Sound.Play("Sounds/SE/Click"); }
    #region Bind & Mapping

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);

        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, names[i], true);
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true);

            if (objects[i] == null)
                Debug.Log($"Failed to bind ({names[i]})");
        }
    }

    protected T Get<T>(int index) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[index] as T;
    }


    #endregion
}
