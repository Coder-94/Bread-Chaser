using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public abstract class BaseScene : MonoBehaviour
{
    #region variables
    public Define.Scene         SceneType { get; protected set; } = Define.Scene.Unknown;
    public string               SceneName { get; private set; }
    public GameObject           Player { get; private set; }
    public float                AreaSize { get; protected set; }

    protected Coroutine         _coroutineIsActive = null;
    protected const int         _MAXMONSTERCOUNT = 5;
    public int                  monsterCount = 0;
    #endregion

    #region MobChecker Struct
    public struct SpawnedMobChecker
    {
        public bool isEnable;
        public Vector3 spawnedPos;
    }

    public SpawnedMobChecker[]  spawnedMobChecker = new SpawnedMobChecker[]
    {
        new SpawnedMobChecker { isEnable = false, spawnedPos = new Vector3(0, 1.65f, 5f) },
        new SpawnedMobChecker { isEnable = false, spawnedPos = new Vector3(-0.82f, 1.65f, 0) },
        new SpawnedMobChecker { isEnable = false, spawnedPos = new Vector3(0.82f, 1.65f, 0) },
        new SpawnedMobChecker { isEnable = false, spawnedPos = new Vector3(-1.62f, 1.65f, 0) },
        new SpawnedMobChecker { isEnable = false, spawnedPos = new Vector3(1.62f, 1.65f, 0) },
        new SpawnedMobChecker { isEnable = false, spawnedPos = new Vector3(0, 2.15f, 0) }
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
        monsterCount = 0;
        SceneName = SceneManager.GetActiveScene().name;

        UnityEngine.Object obj = GameObject.FindAnyObjectByType(typeof(EventSystem));
        if (obj == null)
            Managers.Resource.Instantiate("Prefabs/UI/EventSystem").name = "@EventSystem";

        Player = GameObject.FindGameObjectWithTag("Player");

    }
    #endregion

    #region Spawn Mob In Scene
    protected void MobSpawner(int lessTIme, int maxTime)
    {
        if (monsterCount >= _MAXMONSTERCOUNT || _coroutineIsActive != null)
            return;

        int time = UnityEngine.Random.Range(lessTIme, maxTime);

        _coroutineIsActive = StartCoroutine(SpawnTimer(time));
    }

    IEnumerator SpawnTimer(int time)
    {
       for(int i=0; i<spawnedMobChecker.Length; i++)
       {
            if (spawnedMobChecker[i].isEnable == false)
            {
                GameObject mob = Managers.Resource.Instantiate($"Entity/{SceneName}Mob", null, 5);
                mob.transform.position = spawnedMobChecker[i].spawnedPos;
                spawnedMobChecker[i].isEnable = true;
                monsterCount++;
                break;
            }
        }
        yield return new WaitForSeconds(time);
        _coroutineIsActive = null;
    }
    #endregion

    public abstract void Clear();
    //플레이어 위치 초기화
    //totalLength 초기화
    //몹은 자동초기화 됨
}
