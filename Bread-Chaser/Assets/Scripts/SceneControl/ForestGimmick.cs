using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestGimmick : BaseGimmick
{
    private GameObject beam;
    private GameObject mark;
    private GameObject _currentTargetMob;
    
    private int _targetRailIndex;
    private float _targetX;

    protected override void Init()
    {
        base.Init();

        maxTime = 7.0f;
        maxTime = 9.0f;

        if (warning != null)
        {
            warning.WarnEnded -= GimmickInstantiate;
            warning.WarnEnded += GimmickInstantiate;
        }
    }

    protected override void ActivetGimmick()
    {
        if (_isGimmickActive) return;

        GameObject[] mobs = GameObject.FindGameObjectsWithTag("SmallMob");
        if (mobs == null || mobs.Length == 0) return;

        _isGimmickActive = true;

        _currentTargetMob = mobs[UnityEngine.Random.Range(0, mobs.Length)];

        if (mark == null)
        {
            mark = Managers.Resource.Instantiate("Effect/Mark", _currentTargetMob.transform);
            mark.transform.position = _currentTargetMob.GetComponent<MobController>().targetedPos.transform.position;
            mark.transform.parent = _currentTargetMob.transform;
        }

        float[] rails = Managers.Scene.CurrentScene.railLineX;

        _targetRailIndex = UnityEngine.Random.Range(0, 4);
        _targetX = rails[_targetRailIndex];

        if (warning != null)
        {
            warning.transform.position = new Vector3(_targetX, warning.transform.position.y, warning.transform.position.z);
            warning.WarningInit();
        }
    }

    protected override void GimmickInstantiate()
    {
        if (_currentTargetMob.activeSelf == false || _currentTargetMob == null)
        {
            OnDespawner();
            return;
        }

        beam = Managers.Resource.Instantiate("Area/Forest/ForestGimmickObJ");
        beam.transform.position = new Vector3(_targetX, beam.transform.position.y, beam.transform.position.z);

        ForestGimmickObJ go = beam.GetComponent<ForestGimmickObJ>();
        if (go != null)
        {
            go.Init(_currentTargetMob, OnDespawner);
        }
        else
        {
            _isGimmickActive = false;
        }
    }

    private void OnDespawner()
    {
        if (_currentTargetMob != null)
            _currentTargetMob = null;

        _isGimmickActive = false;
        if (mark != null)
        {
            Managers.Resource.Destroy(mark);
            mark = null;
        }

        if (warning != null && warning.gameObject.activeSelf)
            warning.ToggleVisibility(false);
    }
}
