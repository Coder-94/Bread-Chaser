using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Managers : MonoBehaviour
{
    #region SingleTone
    static Managers s_instance;
    static Managers Instance { get { Init(); return s_instance; } }
    #endregion

    ResourceManager     _resource = new ResourceManager();
    InputManager        _input = new InputManager();
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static InputManager Input { get { return Instance._input; } }

    void Start()
    {
        Init();
	}

    static void Init()
    {
        if (s_instance == null)
        {
			GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();
		}		
	}

    void Update()
    {
        _input.OnUpdate();
    }

    public static void Clear()
    {
        Input.Clear();
    }
}
