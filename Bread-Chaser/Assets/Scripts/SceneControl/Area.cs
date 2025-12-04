using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Area : MonoBehaviour
{
    public float            AreaSize { get; protected set; }
    private float           _disapperRange = -15f;
    private GameObject      _player;

    private PlayerStat _playerStat;
    private PlayerController _playerController;

    private string          _sceneName;
    private  float          _runMag = 20;
    
    private CookieController[] _cookies;

    private void Awake()
    {
        AreaSize = gameObject.GetComponent<BoxCollider>().size.z * gameObject.transform.localScale.z;
        _player = Managers.Game.GetPlayer();

        if (_player != null)
        {
            _playerStat = _player.GetComponent<PlayerStat>();
            _playerController = _player.GetComponent<PlayerController>();
        }

        _cookies = GetComponentsInChildren<CookieController>(true);
        _sceneName = Managers.Scene.CurrentScene.SceneName;
    }

    private void OnEnable()
    {
        ResetCookies();
    }

    private void Update()
    {
        if (_player == null) return;

        float moveSpeed = _player.GetComponent<PlayerStat>().MoveSpeed * _runMag * _playerController.SpeedMultiplier; ;

        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        if (gameObject.transform.position.z + (AreaSize) <= _disapperRange)
        {
            float movedRange = Math.Abs(transform.position.z);
            Managers.Area.SpawnArea(_sceneName, ref Managers.Area.totalLength, movedRange);
            Managers.Resource.Destroy(gameObject);
        }
    }

    public void ResetCookies()
    {
        if (_cookies == null) return;

        foreach (CookieController cookie in _cookies)
        {
            cookie.gameObject.SetActive(true);
        }
    }

    public void Clear() { Destroy(gameObject); }
}
