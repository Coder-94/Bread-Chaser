using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager
{
    public Action<Define.SceneState> StateAction;

    protected Define.GameState  gameState;
    public int                  ScorePoint { get; private set; } = 0;
    float                       _scoreCounter = 0f;
    float                       _magnification = 10f;

    private GameObject          PLAYER;
    private PlayerStat          _playerStat;

    public GameObject GetPlayer() { return PLAYER; }

    public void Init()
    {
        gameState = Define.GameState.Play;
        PLAYER = GameObject.FindGameObjectWithTag("Player");
        if(PLAYER == null)
        {
            Managers.Resource.Instantiate("Entity/Violet");
            PLAYER = GameObject.FindGameObjectWithTag("Player");
        }
        _playerStat = PLAYER.GetComponent<PlayerStat>();
    }

    public void ScoreChecker()
    {
        if (Managers.Scene.CurrentScene.SceneState == Define.SceneState.DefaultPlay || Managers.Scene.CurrentScene.SceneState == Define.SceneState.BossBattle)
        {
            _scoreCounter += 1f * Time.deltaTime * (_playerStat.moveSpeed * _magnification);  //moveSpd per sec
            ScorePoint = Mathf.FloorToInt(_scoreCounter);

            if (ScorePoint != 0 && ScorePoint % 100 == 0)
            {
                Managers.Scene.CurrentScene.SetSceneState(Define.SceneState.LevelUp);
                StateAction.Invoke(Define.SceneState.LevelUp);
            }
        }
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

    public Define.GameState GetGameState() { return gameState; }
    public int ShowScore() { return ScorePoint; }
}
