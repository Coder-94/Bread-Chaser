using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private GameObject _target;
    private PlayerController _targetState;

    [Header("Settings")]
    [SerializeField] private Vector3 _defaultOffset = new Vector3(0, 2.74f, -3.76f);
    [SerializeField] private Vector3 _defaultRotation = new Vector3(15.83f, 0, 0);

    [SerializeField] private Vector3 _atkOffset = new Vector3(0.579f, 0.85f, -1.35f);
    [SerializeField] private Vector3 _atkRotation = new Vector3(-4.7f, -14.9f, 0);

    private float _shakeTilt = 0f;
    private bool _isAtkMode = false;

    void Start()
    {
        Init();
    }

    void Init()
    {
        Camera cam = GetComponent<Camera>();

        float targetAspect = 9f / 16f;
        float currentAspect = (float)Screen.width / (float)Screen.height;
        float aspectRatio = targetAspect / currentAspect;

        if (currentAspect < targetAspect)
        {
            float hFOVInRads = cam.fieldOfView * Mathf.Deg2Rad;
            float vFOVInRads = 2 * Mathf.Atan(Mathf.Tan(hFOVInRads / 2) / aspectRatio);

            cam.fieldOfView = vFOVInRads* Mathf.Rad2Deg;
        }
        cam.rect = new Rect(0, 0, 1, 1);

        if (_target == null)
            _target = Managers.Game.GetPlayer();

        _targetState = _target.GetComponent<PlayerController>();
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        if (_targetState.CurrentState == Define.PlayerStatus.Running ||
            _targetState.CurrentState == Define.PlayerStatus.BackStepping)
        {
            _isAtkMode = false;
        }

        Vector3 finalPos = Vector3.zero;
        Quaternion finalRot = Quaternion.identity;

        if (_isAtkMode)
        {
            finalPos = _target.transform.position + _atkOffset;
            finalRot = Quaternion.Euler(_atkRotation);
        }
        else
        {
            finalPos = new Vector3(_target.transform.position.x, _defaultOffset.y, _defaultOffset.z);
            finalRot = Quaternion.Euler(_defaultRotation);
        }

        transform.position = finalPos;

        Vector3 currentEuler = finalRot.eulerAngles;
        currentEuler.z += _shakeTilt;

        transform.rotation = Quaternion.Euler(currentEuler);
    }

    public void AtkSetting(bool notDeadCheck)
    {
        if (notDeadCheck)
        {
            _isAtkMode = true;
            Debug.Log("시점 변경 ON");
        }
        else
        {
            _isAtkMode = false;
        }
    }


    #region camShake
    public void CamShake(float roughness, float magnitude, float duration)
    {
        StopAllCoroutines();
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

            _shakeTilt = (Mathf.PerlinNoise(tick, 0f) - 0.5f) * magnitude;

            yield return null;
        }

        _shakeTilt = 0f;
    }
    #endregion
}
