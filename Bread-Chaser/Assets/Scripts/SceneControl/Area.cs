using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Area : MonoBehaviour
{
    public float            AreaSize { get; protected set; }
    private float           _disapperRange = -15f;
    private GameObject      _player;
    private void Awake()
    {
        AreaSize = gameObject.GetComponent<BoxCollider>().size.z * gameObject.transform.localScale.z;
        _player = Managers.Scene.CurrentScene.Player;
    }

    private void Update()
    {
        float moveSpeed = _player.GetComponent<PlayerStat>().moveSpeed;

        transform.Translate(Vector3.back * 20f * Time.deltaTime);
        if (gameObject.transform.position.z + (AreaSize) <= _disapperRange)
        {
            float movedRange = Math.Abs(transform.position.z);
            Managers.Area.SpawnArea(Managers.Scene.GetSceneName(Define.Scene.City), ref Managers.Area.totalLength, movedRange);
            Managers.Resource.Destroy(gameObject);
        }
    }
}
