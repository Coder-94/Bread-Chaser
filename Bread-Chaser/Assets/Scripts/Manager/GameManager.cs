using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManager
{
    public int          ScorePoint { get; private set; } = 0;
    float               _scoreCounter = 0f;
    float _magnification = 10f;

    private GameObject  PLAYER;
    private PlayerStat  _playerStat;

    public GameObject GetPlayer() { return PLAYER; }

    public void Init()
    {
        PLAYER = GameObject.FindGameObjectWithTag("Player");
        _playerStat = PLAYER.GetComponent<PlayerStat>();
    }

    public void ScoreChecker()
    {
        if (Managers.Scene.CurrentScene.SceneState != Define.SceneState.Intro)
        {
            _scoreCounter += 1f * Time.deltaTime * (_playerStat.moveSpeed * _magnification);  //moveSpd per sec
            ScorePoint = Mathf.FloorToInt(_scoreCounter);
        }
    }

    public int ShowScore() { return ScorePoint; }
}
