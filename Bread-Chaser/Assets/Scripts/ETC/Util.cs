using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    //어느 구현작업에서든 자주 쓰이는 유틸성 높은 코드 모음

    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }


    public static GameObject FindChild(GameObject go, string name = null, bool again = false)
    {
        Transform transform = FindChild<Transform>(go, name, again);
        if (transform == null)
            return null;
        return transform.gameObject;
    }


    public static T FindChild<T>(GameObject go, string name = null, bool again = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if(again == false)
        {
            for (int i=0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach(T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }
}
