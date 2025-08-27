using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public abstract class BaseScene : MonoBehaviour
{
    #region enums

    protected enum MonsterID
    {
        CityMob = 1,
        CityBoss = 2,
        ForestMob = 3,
        ForestBoss = 4,
        IcycleMob = 5,
        IcycleBoss = 6,
        SpaceMob = 7,
        SpaceBoss = 8,
    }
    #endregion

    #region variables

    public float[] railLineX = new float[4] { -4.7f, -1.7f, 1.7f, 4.7f };

    public Define.Scene             SceneType { get; protected set; } = Define.Scene.Unknown;
    public Define.SceneState        SceneState { get; protected set; }
    public string                   SceneName { get; private set; }
    public GameObject               Player { get; private set; }
    
    public float                    AreaSize { get; protected set; }

    public int                      MonsterCount { get; protected set; } = 0;

    protected Define.PlayerStatus   playerStatus;
    protected Coroutine             _coroutineIsActive = null;
    protected const int             _MAXMONSTERCOUNT = 5;

    #endregion

    #region MobChecker Struct


    public Define.SpawnedMobChecker[] spawnedMobChecker = new Define.SpawnedMobChecker[]
    {
        new Define.SpawnedMobChecker { isSpawned = false, spawnedPos = new Vector3(0, 0.38f, 5f) },
        new Define.SpawnedMobChecker { isSpawned = false, spawnedPos = new Vector3(-0.82f, 0.38f, 5f) },
        new Define.SpawnedMobChecker { isSpawned = false, spawnedPos = new Vector3(0.82f, 0.38f, 5f) },
        new Define.SpawnedMobChecker { isSpawned = false, spawnedPos = new Vector3(-1.62f, 0.38f, 5f) },
        new Define.SpawnedMobChecker { isSpawned = false, spawnedPos = new Vector3(1.62f, 0.38f, 5f) },
        new Define.SpawnedMobChecker { isSpawned = false, spawnedPos = new Vector3(0, 1.65f, 5f) }
    };
    #endregion

    #region Unity Scripts
    void Awake()
    {
        Init();
    }

    #endregion

    #region Init
    protected virtual void Init()
    {
        MonsterCount = 0;
        SceneName = SceneManager.GetActiveScene().name;
        SceneState = Define.SceneState.Intro;
        UnityEngine.Object obj = GameObject.FindAnyObjectByType(typeof(EventSystem));
        if (obj == null)
            Managers.Resource.Instantiate("Prefabs/UI/EventSystem").name = "@EventSystem";

        Player = GameObject.FindGameObjectWithTag("Player");

        Managers.UI.ShowSceneUI<Hp>();
        Managers.UI.ShowSceneUI<Score>();
        Managers.UI.ShowSceneUI<PauseBtn>();

        Managers.UI.ShowPopUpUI<StartTxT>();
    }
    #endregion

    #region Spawn Mob In Scene
    protected void MobSpawner(int id, int lessTIme, int maxTime)
    {
        if (MonsterCount >= _MAXMONSTERCOUNT || _coroutineIsActive != null)
            return;

        int time = UnityEngine.Random.Range(lessTIme, maxTime);

        _coroutineIsActive = StartCoroutine(SpawnTimer(id, time));
    }

    IEnumerator SpawnTimer(int id, int time)
    {
       for(int i=0; i<spawnedMobChecker.Length; i++)
       {
            if (!spawnedMobChecker[i].isSpawned)
            {
                GameObject mob = Managers.Resource.Instantiate($"Entity/{SceneName}/{SceneName}Mob", null, 5);
                mob.transform.position = spawnedMobChecker[i].spawnedPos;

                mob.GetComponent<NormalMobBase>().initPos = spawnedMobChecker[i].spawnedPos;
                mob.GetComponent<NormalMobStat>().SetID(id);

                //Check init
                spawnedMobChecker[i].isSpawned = true;
                mob.GetComponent<NormalMobBase>().SpawnedRoomNumSet(i);

                MobCountController(true);
                break;
            }
        }
        yield return new WaitForSeconds(time);
        _coroutineIsActive = null;
    }
    #endregion

    #region sceneState & scoreCheck
    
    public void SetSceneState(Define.SceneState sceneState)
    {
        SceneState = sceneState;
    }

    #endregion
    public void MobCountController(bool increased )
    {
        if(increased)
            MonsterCount++;
        else
            MonsterCount--;
    }

    public abstract void Clear();
    //플레이어 위치 초기화
    //totalLength 초기화
    //몹은 자동초기화 됨
}
