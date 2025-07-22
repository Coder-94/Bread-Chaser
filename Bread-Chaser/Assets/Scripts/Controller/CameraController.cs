using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject          _target;
    Define.PlayerStatus _status;
    bool                _targetNotDead;
    Vector3             _basePose;
    void Start()
    {
        Init();
    }

    void Init()
    {
        _target = Managers.Scene.CurrentScene.Player;
        _status = _target.GetComponent<PlayerController>().CurrentState;
    }

    void Update()
    {
        CameraControl();
    }

    void CameraControl()
    {

        switch (_status)
        {
            case Define.PlayerStatus.Running:
                RotFixer(15.83f);
                PosFixer(-1.7f, 2.445f, -2.27f);
                break;
            case Define.PlayerStatus.Attacking:
                _targetNotDead = _target.GetComponent<PlayerController>().TargetNotDead;
                if(_targetNotDead == true)
                {
                    //지속 공격용 화면 변환
                }
                break;
        }
    }

    void RotFixer(float x=0, float y=0, float z=0)
    {
        transform.rotation = Quaternion.identity;
        transform.rotation = Quaternion.Euler(x, y, z);
    }

    void PosFixer(float x, float y, float z)
    {
        transform.position = new Vector3(x, y, z);
    }

    public void CamShake(float roughness, float magnitude, float duration)
    {
        StopAllCoroutines();
        _basePose = transform.position;
        StartCoroutine(Shaker(roughness, magnitude, duration));
    }

    IEnumerator Shaker(float roughness, float magnitude, float duration)
    {
        Debug.Log("Shaked!");

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float tick = Time.time * roughness;
            Vector3 offset = new Vector3(
                Mathf.PerlinNoise(tick, 0f) - 0.5f,
                Mathf.PerlinNoise(0f, tick) - 0.5f,
                0f) * magnitude;

            transform.position = _basePose + offset;

            yield return null;
        }
        transform.position = _basePose;
    }
}
