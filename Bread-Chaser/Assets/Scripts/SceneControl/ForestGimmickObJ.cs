using Data;
using System;
using UnityEngine;

public class ForestGimmickObJ : MonoBehaviour
{

    private float hitWidth = 0.5f;

    private Transform _playerTransform;
    private GameObject _linkedMob;
    private Action _onDespawn;

    public void Init(GameObject mob, Action onDespawn) 
    {
        Managers.Sound.Play("SE/ForestGimmickPopUp");
        _linkedMob = mob;
        _onDespawn = onDespawn;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            _playerTransform = player.transform;
    }

    void Update()
    {
        if (_linkedMob.activeSelf == false ||  _linkedMob == null)
        {
            SelfDestroy();
            return;
        }

        if (_playerTransform != null)
        {
            if (Mathf.Abs(_playerTransform.position.x - transform.position.x) <= hitWidth)
            {
                _playerTransform.GetComponent<PlayerStat>()?.OnPlAttacked(gameObject, 3f);
            }
        }
    }

    private void SelfDestroy()
    {
        _onDespawn?.Invoke();

        Managers.Resource.Destroy(gameObject);
    }
}
