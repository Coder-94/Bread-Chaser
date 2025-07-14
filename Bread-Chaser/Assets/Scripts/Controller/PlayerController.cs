using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Define;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : PlayerBase
{

    #region player movement

    protected override void PlayerActor(Define.TouchEvent evt)
    {
        switch (CurrentState)
        {
            case Define.PlayerStatus.LockOning:
                LockOn(evt);
                break;
        }
    }

    protected override void PlayerControl() 
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
                LockOn();
                break;
            case Define.PlayerStatus.Jumping:

                break;
        }
    }

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

    #region jump

    protected override void Jump()
    {
        if (CurrentState == Define.PlayerStatus.Running)
            StartCoroutine(JumpCoroutine(1.5f));
    }

    IEnumerator JumpCoroutine(float cooldownTime)
    {
        CurrentState = Define.PlayerStatus.Jumping;
        _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerJump]);

        int random = UnityEngine.Random.Range(1, 4);

        Managers.Sound.Play($"SE/JumpVoice{random}");
        _movementQueue.Enqueue(JumpRB);

        yield return new WaitForSeconds(cooldownTime);
    }

    void JumpRB()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
    }

    #endregion

    #region targetting

    protected void LockOn(Define.TouchEvent evt)
    {
        CurrentState = Define.PlayerStatus.LockOning;


    }

    #endregion

    #region atk


    protected override void Attack()
    {
        if (_target != null)
        {

        }

        if (CurrentState == Define.PlayerStatus.Running)
        {
            _originPos = gameObject.transform.position;
            _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerAtk]);
            CurrentState = Define.PlayerStatus.Attacking;
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
    #endregion

    #region backstep

    protected override void BackStep()
    {
        Debug.Log("¹é½ºÅÇ!");

        _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerBackStep]);
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

    #endregion

    #region Anim Events

    public void OnAttack()
    {
        NormalMobStat targetStat = _target.GetComponent<NormalMobStat>();
        Managers.Sound.Play($"SE/Hit");
        targetStat.OnAttacked(_stat.Atk);
    }

    public void StatusInit()
    {
        CurrentState = Define.PlayerStatus.Running;
    }

    #endregion

}