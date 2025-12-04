using UnityEngine;

public class ObstacleController : MonoBehaviour
{

    GameObject player;

    private void Start()
    {
        player = Managers.Game.GetPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        Define.SceneState sceneState = Managers.Scene.CurrentScene.SceneState;

        if (other.gameObject.layer == (int)Define.Layer.Player && sceneState != Define.SceneState.Ending && sceneState != Define.SceneState.Intro) 
        {
            player.GetComponent<PlayerStat>().OnPlAttacked(gameObject);
        }
    }
}
