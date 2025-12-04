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
    ResourceManager     _resource = new();
    PoolManager         _pool = new();
    InputManager        _input = new();
    UIManager           _ui = new();
    SceneManagerEX      _scene = new();
    AreaManager         _area = new();
    DataManager         _data = new();
    SoundManager        _sound = new();
    GameManager         _game = new();

    public static   ResourceManager     Resource { get { return Instance._resource; } }
    public static   PoolManager         Pool { get { return Instance._pool; } }
    public static   InputManager        Input { get { return Instance._input; } }
    public static   UIManager           UI { get { return Instance._ui; } }
    public static SceneManagerEX        Scene { get { return Instance._scene; } }
    public static AreaManager           Area { get { return Instance._area; } }
    public static DataManager           Data { get { return Instance._data; } }
    public static SoundManager          Sound { get { return Instance._sound; } }
    public static GameManager           Game { get { return Instance._game; } }
    #endregion

    #region Start & Update
    void Start()
    {
        Init();

    }

    void Update()
    {
        _input.OnUpdate();

        _game.OnUpdate();
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
            s_instance._sound.Init();
            s_instance._game.Init();
        }		
	}


    public static void Clear()
    {
        Area.Clear();
        Sound.Clear();
        Input.Clear();
        Scene.Clear();
        UI.Clear();
        Game.Clear();
        Pool.Clear();
    }
    #endregion
}
