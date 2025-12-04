using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager
{
    public Action<Define.SceneState> StateAction;

    protected Define.GameState  gameState;

    public int                  ScorePoint { get; private set; } = 0;
    float                       _scoreCounter = 0f;
    float                       _magnification = 10f;

    int                         _nextLevelUpScore = 1000;
    const int                   LEVELUPCUT = 1000;
    private int                 _pendingLevelUps = 0;

    private GameObject          PLAYER;
    private PlayerStat          _playerStat;
    private PlayerController    _playerCont;
    public int                  BossBattleScoreCut { get; private set; } = 10; //5900
    public float                StageStartScore { get; private set; } = 0f;
    public bool                 IsBossBattleStarted { get; private set; } = false;

    public int                  CookieNum { get; private set; } = 0;
    public int                  BossExp { get; private set; } = 0;
    public float                PlayTime { get; private set; } = 0f;

    public Vector3              PlStartPos { get; private set; }


    public void SetBossExp(int value) { BossExp = value; }
    public void AddCookieNum() { CookieNum++; }
    public GameObject GetPlayer() { return PLAYER; }
    public Define.GameState GetGameState() { return gameState; }
    public int ShowScore() { return ScorePoint; }

    public void SearchPlayer() 
    { 
        PLAYER = GameObject.FindGameObjectWithTag("Player"); 

        if(PLAYER != null)
        {
            _playerStat = PLAYER.GetComponent<PlayerStat>();
            _playerCont = PLAYER.GetComponent<PlayerController>();
        }
            
    }

    public void Init()
    {
        gameState = Define.GameState.Play;
        PLAYER = GameObject.FindGameObjectWithTag("Player");
        PlStartPos = new Vector3(-1.7f, 0.275f, 0);

        if(PLAYER == null)
        {
            SearchPlayer();
        }
    }

    

    public void OnUpdate()
    {
        Define.SceneState currentSceneState = Managers.Scene.CurrentScene.SceneState;

        if (gameState == Define.GameState.Play &&
           (currentSceneState == Define.SceneState.DefaultPlay ||
            currentSceneState == Define.SceneState.BossBattle))
        {
            PlayTime += Time.deltaTime;
        }

        ScoreChecker();
    }

    private void ScoreChecker()
    {
        Define.SceneState currentSceneState = Managers.Scene.CurrentScene.SceneState;

        if (_playerStat != null)
        {
            if (_playerCont.CurrentState == Define.PlayerStatus.Die)
                return;

            if (gameState == Define.GameState.Play && currentSceneState == Define.SceneState.DefaultPlay || currentSceneState == Define.SceneState.BossBattle)
            {
                _scoreCounter += 1f * Time.deltaTime * (Mathf.Sqrt(_playerStat.MoveSpeed) * _magnification);  //moveSpd per sec
                ScorePoint = Mathf.FloorToInt(_scoreCounter);

                if(_playerCont.CurrentState == Define.PlayerStatus.Running)
                    LvUpLogic();

                if (!IsBossBattleStarted && currentSceneState == Define.SceneState.DefaultPlay && (ScorePoint - (int)StageStartScore) >= BossBattleScoreCut)
                {
                    IsBossBattleStarted = true;

                    StateAction?.Invoke(Define.SceneState.BossBattle);
                    Managers.Scene.CurrentScene.SetSceneState(Define.SceneState.BossBattle);
                    Debug.Log("Boss Battle Started!");
                }

            }
            else if (currentSceneState == Define.SceneState.Ending)
            {
                LvUpLogic();
            }
        }
            
    }

    void LvUpLogic()
    {
        while (ScorePoint >= _nextLevelUpScore)
        {
            _pendingLevelUps++;
            TryOpenLevelUpPopup();
            _nextLevelUpScore += LEVELUPCUT;
        }
    }

    public void CalculFinalScore()
    {
        EndingScene sceneUI = Managers.UI.ShowSceneUI<EndingScene>();

        sceneUI.PanelInit(PlayTime, CookieNum, BossExp);
    }

    public void TryOpenLevelUpPopup()
    {
        if (_pendingLevelUps > 0 && gameState == Define.GameState.Play)
        {
            _pendingLevelUps--;

            StateAction?.Invoke(Define.SceneState.LevelUp);
        }
    }

    public void AddScore(float amount) 
    {
        _scoreCounter += amount;
        ScorePoint = Mathf.FloorToInt(_scoreCounter);
    }

    public void SetGameState(Define.GameState currentState) 
    {
        if (gameState == currentState)
        { Debug.Log($"{currentState} is already same!"); return; }

        if (currentState == Define.GameState.Play)
            Time.timeScale = 1f;
        else if(currentState == Define.GameState.Paused)
            Time.timeScale = 0f;

        gameState = currentState;
    }

    public void ClearMob()
    {
        GameObject[] mobs;
        mobs = GameObject.FindGameObjectsWithTag("SmallMob");

        for (int i = 0; i < mobs.Length; i++)
            mobs[i].GetComponent<Stat>().RightKill();
    }

    public void Clear()
    {
        StageStartScore = _scoreCounter;

        if(BossExp == 0)
        {
            StageStartScore = 0;
            _scoreCounter = 0;
            ScorePoint = 0;
            _nextLevelUpScore = 0;
        } 

        CookieNum = 0;
        BossExp = 0;
        PlayTime = 0f;

        IsBossBattleStarted = false;

        if (PLAYER != null)
        {
            PLAYER.transform.position = PlStartPos;
            PLAYER.GetComponent<PlayerController>().Clear();

            if (_playerCont.CurrentState == Define.PlayerStatus.Die)
                PLAYER = null;
        }
            
    }
}
