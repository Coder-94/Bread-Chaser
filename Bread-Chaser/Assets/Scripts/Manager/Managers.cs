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

    #region Managers
    ResourceManager     _resource = new ResourceManager();
    PoolManager         _pool = new PoolManager();
    InputManager        _input = new InputManager();
    UIManager           _ui = new UIManager();
    SceneManagerEX      _scene = new SceneManagerEX();
    AreaManager         _area = new AreaManager();
    DataManager         _data = new DataManager();
    public static   ResourceManager   Resource { get { return Instance._resource; } }
    public static   PoolManager       Pool { get { return Instance._pool; } }
    public static   InputManager      Input { get { return Instance._input; } }
    public static   UIManager         UI { get { return Instance._ui; } }
    public static SceneManagerEX      Scene { get { return Instance._scene; } }
    public static AreaManager         Area { get { return Instance._area; } }
    public static DataManager         Data { get { return Instance._data; } }
    #endregion

    #region Start & Update
    void Start()
    {
        Init();

    }

    void Update()
    {
        _input.OnUpdate();
    }
    #endregion

    #region Initialize & Clear
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

            s_instance._data.Init();
            s_instance._pool.Init();
		}		
	}


    public static void Clear()
    {
        Input.Clear();

        Pool.Clear();
    }
    #endregion
}
