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
        if (other.gameObject.layer == (int)Define.Layer.Player) 
        {
            player.GetComponent<PlayerStat>().OnPlAttacked(gameObject);
        }
    }
}
