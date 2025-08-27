using UnityEngine;

public class GameManager
{
    public int          ScorePoint { get; private set; } = 0;
    float               _scoreCounter = 0f;

    private GameObject PLAYER;

    public GameObject GetPlayer() { return PLAYER; }

    public void Init()
    {
        PLAYER = GameObject.FindGameObjectWithTag("Player");
    }

    public void ScoreChecker()
    {
        if (Managers.Scene.CurrentScene.SceneState != Define.SceneState.Intro)
        {
            _scoreCounter += 1f * Time.deltaTime * PLAYER.GetComponent<PlayerStat>().moveSpeed;  //moveSpd per sec
            ScorePoint = Mathf.FloorToInt(_scoreCounter);
        }
    }

    public int ShowScore() { return ScorePoint; }
}
