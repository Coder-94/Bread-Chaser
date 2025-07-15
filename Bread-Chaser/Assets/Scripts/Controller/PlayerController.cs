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

    protected override void PlayerControl(Define.TouchEvent evt, Vector2? dist = null)
    {
        switch (CurrentState)
        {
            case Define.PlayerStatus.Running:
                Running(evt);
                break;
            case Define.PlayerStatus.LockOning:
                LockOn(evt, dist);
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
        _cursor = Managers.Resource.Instantiate("UI/LockOn");
        _cursorRect = _cursor.transform.Find("TargetCursor").GetComponent<RectTransform>();

        Touch touch = Input.GetTouch(0);
        Vector2 currentTouchPos = touch.position;
        _lastTouchPos = currentTouchPos;
        _isTouching = true;
    }

    protected void LockOn(Define.TouchEvent evt, Vector2? dist)
    {
       if(_cursor != null)
        {
            switch (evt)
            {
                case Define.TouchEvent.Holding:
                    if (Input.touchCount > 0)
                    {
                        Touch touch = Input.GetTouch(0);
                        Vector2 currentTouchPos = touch.position;

                        if (_isTouching)
                        {
                            Vector2 delta = currentTouchPos - _lastTouchPos;
                            _lastTouchPos = currentTouchPos;
                            _cursorRect.anchoredPosition += delta * 3.5f;
                        }
                    }
                    break;
                case Define.TouchEvent.HoldedFingerReleased:
                    _isTouching = false;
                    Managers.Resource.Destroy(_cursor);
                    CurrentState = Define.PlayerStatus.Running;
                    break;
            }

        }
    }

    protected override void Attack()
    {
        if (_target != null)
        {
            _originPos = gameObject.transform.position;
            _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerAtk]);
            CurrentState = Define.PlayerStatus.Attacking;
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
        if (_target != null && _target.activeInHierarchy)
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
        else
        {
            BackStep();
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