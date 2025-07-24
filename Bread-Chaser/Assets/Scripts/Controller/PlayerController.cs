using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.DeviceSimulation;
using UnityEngine;
using static Define;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : PlayerBase
{

    #region player control

    //input control ======================================================================================

    protected override void PlayerControl(Define.TouchEvent evt)
    {
        switch (CurrentState)
        {
            case Define.PlayerStatus.Running:
                Running(evt);
                break;
            case Define.PlayerStatus.LockOning:
                LockOn(evt);
                break;
        }
    }

    //run ======================================================================================
    void Running(Define.TouchEvent evt)
    {
        switch (evt)
        {
            case Define.TouchEvent.UpSwipe:
                CurrentState = Define.PlayerStatus.Jumping;
                break;
            case Define.TouchEvent.StartHolding:
                CurrentState = Define.PlayerStatus.LockOning;
                break;
        }
    }

    //lock on ======================================================================================
    protected void LockOn(Define.TouchEvent evt)
    {

        if(evt == Define.TouchEvent.HoldedFingerReleased)
        {
            _target = _targeting.GetComponent<LockOn>().GetTarget();

            if (_target == null)
                CurrentState = Define.PlayerStatus.Running;
            else
                CurrentState = Define.PlayerStatus.Attacking;

            _targeting = null;
        }
    }

    //atk ======================================================================================

    protected void Atk()
    {
        if (_target == null || _target.GetComponent<NormalMobStat>().Hp <= 0)
        {
            CurrentState = Define.PlayerStatus.BackStepping;
            _target = null;
            TargetNotDead = false;
        }
        else 
        {
            TargetNotDead = true;
        }

    }

    #endregion

    #region player act

    //state control ======================================================================================
    protected override void PlayerActor() 
    {
        switch (CurrentState)
        {
            case Define.PlayerStatus.Running:
                break;
            case Define.PlayerStatus.Attacking:
                Atk();
                break;
            case Define.PlayerStatus.BackStepping:
                break;
            case Define.PlayerStatus.LockOning:
                break;
            case Define.PlayerStatus.Jumping:
                break;
        }
    }
    #endregion

    #region rb

    //rb control ======================================================================================
    protected override void RbControl()
    {
        switch (CurrentState)
        {
            case Define.PlayerStatus.Attacking:
                AttackRB();
                break;
            case Define.PlayerStatus.BackStepping:
                BackStepRB();
                break;
        }

        //Mission Delay =====================================================================================
        while (_movementQueue.Count > 0)
        {
            Action action = _movementQueue.Dequeue();
            action?.Invoke();
        }
    }

    //rb atk ======================================================================================
    void AttackRB()
    {
        if (_target != null)
        {
            Vector3 targetPos = _target.transform.position;
            Vector3 currentPos = _rb.position;

            targetPos.y = currentPos.y;

            Vector3 direction = (targetPos - currentPos).normalized;

            float dist = Vector3.Distance(currentPos, targetPos);

            if (dist > 0.01f)
            {
                float moveStep = 15f * Time.fixedDeltaTime;
                float moveAmount = Mathf.Min(moveStep, dist);

                Vector3 move = direction * moveAmount;
                _rb.MovePosition(currentPos + move);
            }
        }
    }

    // rb backstep ======================================================================================
    void BackStepRB()
    {
        Vector3 currentPos = _rb.position;
        Vector3 dir = _originPos - currentPos;
        dir.y = 0;

        float dist = dir.magnitude;

        if (dist > 0.01f)
        {
            float moveStep = 15f * Time.fixedDeltaTime;
            float moveAmount = Mathf.Min(moveStep, dist);

            Vector3 moveDir = dir.normalized * moveAmount;
            _rb.MovePosition(currentPos + moveDir);
        }
        else
            CurrentState = Define.PlayerStatus.Running;
    }
    #endregion

    #region jump

    //jump rb ======================================================================================
    protected override void JumpRB()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
    }

    #endregion

    #region anim Events

    public void OnAttack()
    {
        NormalMobStat targetStat = _target.GetComponent<NormalMobStat>();
        Managers.Sound.Play($"SE/Hit");
        Camera.main.GetComponent<CameraController>().CamShake(10f, 1f, 0.4f);
        targetStat.OnAttacked(_stat.Atk);
        _punchEffect.GetComponent<ParticleSystem>().Play();
    }

    
    #endregion

}