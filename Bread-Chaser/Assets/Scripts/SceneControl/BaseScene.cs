using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using UnityEngineInternal;

public class BaseScene : MonoBehaviour
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
    public string                   SceneName { get; protected set; }
    public float                    AreaSize { get; protected set; }
    public int                      MonsterCount { get; protected set; } = 0;

    public int                      MobKillCount; //{ get; protected set; }
    public GameObject               CurrentBoss { get; protected set; }
    protected Define.SceneState     _sceneState;
    protected int                   _bossId;
    protected int                   _mobid;
    protected Define.PlayerStatus   playerStatus;

    protected Coroutine             _coroutineIsActive = null;
    protected const int             _MAXMONSTERCOUNT = 5;


    protected bool                  _alreadyEnd = false;
    public Define.SceneState SceneState 
    {
        get { return _sceneState; }
        set
        {
            if (_sceneState == Define.SceneState.LevelUp && value == Define.SceneState.DefaultPlay || value == Define.SceneState.BossBattle)
                StartCoroutine(Managers.Game.GetPlayer().GetComponent<PlayerStat>().InvincibleProcess(false, 0.5f));
            if (_sceneState == value) return;

            _sceneState = value;

            if (_sceneState == Define.SceneState.Ending) 
            {
                if (!_alreadyEnd)
                {
                    Managers.Game.CalculFinalScore();
                    _alreadyEnd = true;
                }
            }

        }
    }
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
    protected void Awake()
    {
        Init();
    }

    protected virtual void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            Managers.UI.ShowPopUpUI<GameQuitPopUp>();

        BossAtk();

        if (SceneState != Define.SceneState.Intro && SceneState != Define.SceneState.Ending)
            MobSpawner(_mobid, 3, 7);
    }
    #endregion

    #region Init
    protected virtual void Init()
    {
        Managers.Game.StateAction -= LevelUp;
        Managers.Game.StateAction += LevelUp;
        Managers.Game.StateAction -= BossStage;
        Managers.Game.StateAction += BossStage;

        if(Managers.Game.GetPlayer() == null && SceneType != Define.Scene.Title)
        {
            Managers.Resource.Instantiate("Entity/Violet");
            Managers.Game.SearchPlayer();
        }

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
        Managers.UI.ShowPopUpUI<FadeOutPopUp>();
    }
    #endregion

    #region Spawn Mob In Scene
    protected void MobSpawner(int id, int lessTIme, int maxTime)
    {
        if (_coroutineIsActive != null)
            return;
        
        _coroutineIsActive = StartCoroutine(SpawnCycleRoutine(id, lessTIme, maxTime));
    }

    IEnumerator SpawnCycleRoutine(int id, int minTime, int maxTime)
    {
        while (true)
        {
            float waitTime = UnityEngine.Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);

            PlayerController plCon = Managers.Game.GetPlayer().GetComponent<PlayerController>();
            if (plCon.BrakeForEnd)
            {
                _coroutineIsActive = null;
                yield break;
            }

            if (SceneState == Define.SceneState.DefaultPlay || SceneState == Define.SceneState.BossBattle)
            {
                TrySpawnMob(id);
            }
        }
    }

    void TrySpawnMob(int id)
    {
        if (MonsterCount >= _MAXMONSTERCOUNT)
            return;

        for (int i = 0; i < spawnedMobChecker.Length; i++)
        {
            if (!spawnedMobChecker[i].isSpawned)
            {
                GameObject mob = Managers.Resource.Instantiate($"Entity/{SceneName}/{SceneName}Mob", null, 5);
                mob.transform.position = spawnedMobChecker[i].spawnedPos;

                mob.GetComponent<MobController>().initPos = spawnedMobChecker[i].spawnedPos;
                mob.GetComponent<MobStat>().SetID(id);

                spawnedMobChecker[i].isSpawned = true;
                mob.GetComponent<MobController>().SpawnedRoomNumSet(i);

                MobCountController(true);

                break;
            }
        }
    }

    protected virtual void BossStage(Define.SceneState sceneState) 
    {
        if (sceneState == Define.SceneState.BossBattle)
        {
            SceneState = Define.SceneState.BossBattle;

            GameObject bossMob = Managers.Resource.Instantiate($"Entity/{SceneName}/{SceneName}Boss", null, 1);
            bossMob.transform.position = spawnedMobChecker[5].spawnedPos;

            bossMob.GetComponent<MobController>().initPos = spawnedMobChecker[5].spawnedPos;
            bossMob.GetComponent<MobStat>().SetID(_bossId);

            spawnedMobChecker[5].isSpawned = true;
            bossMob.GetComponent<MobController>().SpawnedRoomNumSet(5);
            Managers.UI.ShowSceneUI<MobHp>();

            CurrentBoss = bossMob;
        }

    }

    public void BossStaMobKillCount() { MobKillCount++; }

    protected void BossAtk() 
    {
        int countCut = 7;

        if(MobKillCount >= countCut)
        {
            MobKillCount -= countCut;
            Managers.UI.ShowPopUpUI<BossAtkBtn>();
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

    public virtual void Clear()
    {
        Managers.Game.StateAction -= LevelUp;
        Managers.Game.StateAction -= BossStage;
    }

    public virtual void ForestGimmick() { }
    public virtual void IceLandGimmick() { }
    public virtual void UniverseGimmick() { }
}
