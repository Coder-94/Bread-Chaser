using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject          _target;
    Vector3             _basePose;
    PlayerController    _targetState;
    void Start()
    {
        Init();
    }

    void Init()
    {
        _target = Managers.Game.GetPlayer();
        _targetState = _target.GetComponent<PlayerController>();
        DefaultSetting();
    }

    private void Update()
    {
        SideMoveCamControl();
    }

    private void LateUpdate()
    {
        if (_targetState.CurrentState == Define.PlayerStatus.Running || _targetState.CurrentState == Define.PlayerStatus.BackStepping)
            DefaultSetting();
    }

    void SideMoveCamControl()
    {
        if (_targetState.CurrentState == Define.PlayerStatus.LeftMoving || _targetState.CurrentState == Define.PlayerStatus.RightMoving)
            DefaultSetting();
    }


    #region camSetting

    public void DefaultSetting()
    {
        RotFixer(15.83f);
        PosFixer(_target.transform.position.x, 2.74f, -3.76f);
    }

    public void AtkSetting(bool notDeadCheck)
    {
        if (notDeadCheck)
        {
            Debug.Log("시점 변경 on");
            RotFixer(-4.7f, -14.9f);
            PosFixer(_target.transform.position.x + 0.579f,
            _target.transform.position.y + 0.85f,
            _target.transform.position.z - 1.35f);
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
    #endregion

    #region camShake
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
    #endregion
}
