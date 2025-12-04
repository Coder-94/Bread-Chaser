using UnityEngine;

public class CookieController : MonoBehaviour
{
    int cookiePoint = 20;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == (int)Define.Layer.Player)
        {
            Managers.Game.AddScore(cookiePoint);
            Managers.Game.AddCookieNum();
            Managers.Sound.Play("SE/Cookie");
            gameObject.SetActive(false);
        }
    }
}
