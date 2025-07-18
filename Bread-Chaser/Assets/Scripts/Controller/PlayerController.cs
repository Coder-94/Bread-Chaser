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
            case Define.PlayerStatus.Attacking:
                LockOn(evt);
                break;
        }
    }

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

    protected override void StartLockOn()
    {
        if(_targeting == null)
            _targeting = Managers.UI.ShowPopUpUI<LockOn>().gameObject;

        Touch touch = Input.GetTouch(0);
        Vector2 currentTouchPos = touch.position;

        _targeting.GetComponent<LockOn>().SetFirstTouch(currentTouchPos);
    }

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

    protected override void Attack()
    {
        if (_target != null)
        {
            _originPos = gameObject.transform.position;
            _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerAtk]);
        }
    }

    protected override void BackStep()
    {
        Debug.Log("¹é½ºÅÇ!");

        _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerBackStep]);
    }
    #endregion

    #region player act
    protected override void PlayerActor() 
    {
        switch (CurrentState)
        {
            case Define.PlayerStatus.Running:
                break;
            case Define.PlayerStatus.Attacking:
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

    void AttackRB()
    {
        if (_target != null)
        {
            Vector3 targetPos = _target.transform.position;
            float targetZ = targetPos.z - 0.5f;
            Vector3 currentPos = _rb.position;

            Vector3 direction = (targetPos - currentPos);
            direction.y = 0;

            float dist = Mathf.Abs(currentPos.z - targetZ);

            if (dist > 0.01f)
            {
                float moveStep = 15f * Time.fixedDeltaTime;
                float moveAmount = Mathf.Min(moveStep, dist);

                Vector3 move = Vector3.forward * Mathf.Sign(targetZ - currentPos.z) * moveAmount;
                _rb.MovePosition(currentPos + move);
            }
        }
    }

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

    protected override void Jump()
    {
        if(!_isJumping)
        {
            _isJumping = true;
            _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerJump]);

            int random = UnityEngine.Random.Range(1, 4);
            
            Managers.Sound.Play($"SE/JumpVoice{random}");
            _movementQueue.Enqueue(JumpRB);
        }
    }

    void JumpRB()
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
        targetStat.OnAttacked(_stat.Atk);
    }

    public void StatusInit()
    {
        //CurrentState = Define.PlayerStatus.Running;
    }

    #endregion

}