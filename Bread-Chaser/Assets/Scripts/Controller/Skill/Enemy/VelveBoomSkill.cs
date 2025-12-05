using UnityEngine;

public class VelveBoomSkill : MonoBehaviour
{
    private bool _isSuccess;

    public void Init(bool isSuccess)
    {
        Managers.Sound.Play("SE/VelveBooom");
        Camera.main.GetComponent<CameraController>().CamShake(5, 50, 0.3f);
        _isSuccess = isSuccess;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isSuccess)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Hit");
                PlayerStat plStat = other.GetComponent<PlayerStat>();
                if (plStat != null)
                {
                    plStat.TrueDmgAttacked(plStat.Hp / 3.0f);
                }
            }
        }
        else if (!_isSuccess)
        {
            if (other.gameObject.layer == (int)Define.Layer.Boss)
            {
                Debug.Log("HitHer");
                MobStat mobStat = other.GetComponent<MobStat>();
                if (mobStat != null)
                {
                    mobStat.VelveFailure();
                }
            }
        }
    }
}
