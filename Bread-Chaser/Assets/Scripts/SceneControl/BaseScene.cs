using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngineInternal;

public abstract class BaseScene : MonoBehaviour
{
    #region enums

    protected enum MonsterID
    {
        CityMob = 1,
        IcycleMob = 2,
        ForestMob = 3,
        SpaceMob = 4,
        CityBoss = 5,
        IcycleBoss = 6,
        ForestBoss = 7,
        SpaceBoss = 8,
    }
    #endregion

    #region variables

    public float[] railLineX = new float[4] { -4.7f, -1.7f, 1.7f, 4.7f };

    public Define.Scene             SceneType { get; protected set; } = Define.Scene.Unknown;
    public Define.SceneState        SceneState { get; protected set; }
    public string                   SceneName { get; private set; }
    public float                    AreaSize { get; protected set; }
    public int                      MonsterCount { get; protected set; } = 0;

    protected int                   _bossId;
    protected Define.PlayerStatus   playerStatus;
    protected Coroutine             _coroutineIsActive = null;
    protected const int             _MAXMONSTERCOUNT = 5;

    #endregion

    #region MobChecker Struct


    public Define.SpawnedMobChecker[] spawnedMobChecker = new Define.SpawnedMobChecker[]
    {
        new() { isSpawned = false, spawnedPos = new Vector3(0, 0.38f, 8f) },
        new() { isSpawned = false, spawnedPos = new Vector3(-0.82f, 0.38f, 8f) },
        new() { isSpawned = false, spawnedPos = new Vector3(0.82f, 0.38f, 8f) },
        new() { isSpawned = false, spawnedPos = new Vector3(-1.62f, 0.38f, 8f) },
        new() { isSpawned = false, spawnedPos = new Vector3(1.62f, 0.38f, 8f) },
        new() { isSpawned = false, spawnedPos = new Vector3(0, 1.65f, 8f) }
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
        Managers.Game.StateAction -= LevelUp;
        Managers.Game.StateAction += LevelUp;
        Managers.Game.StateAction -= BossStage;
        Managers.Game.StateAction += BossStage;

        MonsterCount = 0;
        SceneName = SceneManager.GetActiveScene().name;
        SceneState = Define.SceneState.Intro;
        UnityEngine.Object obj = GameObject.FindAnyObjectByType(typeof(EventSystem));
        if (obj == null)
            Managers.Resource.Instantiate("Prefabs/UI/EventSystem").name = "@EventSystem";


        Managers.UI.ShowSceneUI<Hp>();
        Managers.UI.ShowSceneUI<Score>();
        Managers.UI.ShowSceneUI<PauseBtn>();
        Managers.UI.ShowSceneUI<SkillBtn>();
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

                mob.GetComponent<MobController>().initPos = spawnedMobChecker[i].spawnedPos;
                mob.GetComponent<NormalMobStat>().SetID(id);

                //Check init
                spawnedMobChecker[i].isSpawned = true;
                mob.GetComponent<MobController>().SpawnedRoomNumSet(i);

                MobCountController(true);
                break;
            }
        }
        yield return new WaitForSeconds(time);
        _coroutineIsActive = null;
    }
    
    protected virtual void BossStage(Define.SceneState sceneState) 
    {
        if (sceneState == Define.SceneState.BossBattle)
        {
            SceneState = Define.SceneState.BossBattle;

            GameObject bossMob = Managers.Resource.Instantiate($"Entity/{SceneName}/{SceneName}Boss", null, 1);
            bossMob.transform.position = spawnedMobChecker[5].spawnedPos;

            bossMob.GetComponent<MobController>().initPos = spawnedMobChecker[5].spawnedPos;
            bossMob.GetComponent<NormalMobStat>().SetID(_bossId);

            spawnedMobChecker[5].isSpawned = true;
            bossMob.GetComponent<MobController>().SpawnedRoomNumSet(5);
        }

    }

    #endregion

    #region sceneState
    
    public void SetSceneState(Define.SceneState sceneState) { SceneState = sceneState; }

    void LevelUp(Define.SceneState sceneState) 
    { 
        if(sceneState == Define.SceneState.LevelUp)
        {
            Managers.Scene.CurrentScene.SetSceneState(Define.SceneState.LevelUp);
            Managers.UI.ShowPopUpUI<LevelUp>();
            Managers.Game.SetGameState(Define.GameState.Paused);
        }
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
