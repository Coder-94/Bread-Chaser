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
            case Define.PlayerStatus.Attacking:
                Attacking(evt);
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
            case Define.TouchEvent.LeftSwipe:
                SideMove(evt);
                break;
            case Define.TouchEvent.RightSwipe:
                SideMove(evt);
                break;
            case Define.TouchEvent.StartHolding:
                CurrentState = Define.PlayerStatus.LockOning;
                break;
        }
    }

    //sideMove ======================================================================================
    void SideMove(Define.TouchEvent evt)
    {
        if(evt == Define.TouchEvent.LeftSwipe && _railPos != Define.PLRailPos.FirstRail)
        {
            _railPos--;
            CurrentState = Define.PlayerStatus.LeftMoving;
        }
        else if(evt == Define.TouchEvent.RightSwipe && _railPos != Define.PLRailPos.FourthRail)
        {
            _railPos++;
            CurrentState = Define.PlayerStatus.RightMoving;
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

    void Attacking(Define.TouchEvent evt)
    {
        if (evt == Define.TouchEvent.DownSwipe)
            CurrentState = Define.PlayerStatus.BackStepping;

        if (TargetNotDead)
        {
            if (!_touchBlock && evt == Define.TouchEvent.Tap)
            {
                int rand = UnityEngine.Random.Range(0, 2);

                if(rand == 0)
                    _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerLeftAtk]);
                else
                    _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerRightAtk]);

                Camera.main.GetComponent<CameraController>().CamShake(10f, 1f, 0.2f);
                _touchBlock = true;
            }
        }
    }

    protected void Atk()
    {
        if (_target != null)
        {
            Vector3 dir = _target.transform.position - transform.position;
            dir.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        }

        if (_target == null || _target.GetComponent<NormalMobStat>().Hp <= 0)
        {
            CurrentState = Define.PlayerStatus.BackStepping;
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
            case Define.PlayerStatus.LeftMoving:
                SideMoveRB();
                break;
            case Define.PlayerStatus.RightMoving:
                SideMoveRB();
                break;
        }

        //Mission Delay =====================================================================================
        while (_movementQueue.Count > 0)
        {
            Action action = _movementQueue.Dequeue();
            action?.Invoke();
        }
    }

    //sideMoving ======================================================================================
    void SideMoveRB()
    {
        Vector3 dest = new Vector3(Managers.Scene.CurrentScene.railLineX[(int)_railPos], 0, 0);
        Vector3 currentPos = _rb.position;

        Vector3 dir = dest - currentPos;

        float dist = dir.magnitude;

        if (Math.Abs(dir.x) > 0.01f)
        {
            float moveStep = 15f * Time.fixedDeltaTime;
            float moveAmount = Mathf.Min(moveStep, dist);

            Vector3 moveDir = dir.normalized * moveAmount;
            _rb.MovePosition(currentPos + moveDir);

            Debug.Log($"railX: {Managers.Scene.CurrentScene.railLineX[(int)_railPos]}, rbPos: {_rb.position.x}");
        }
        else
            CurrentState = Define.PlayerStatus.Running;
    }

    //rb atk ======================================================================================
    void AttackRB()
    {
        if (_target != null)
        {
            Vector3 targetPos = _target.transform.position;
            Vector3 currentPos = _rb.position;

            targetPos.y = currentPos.y;
            targetPos.z -= 1f;

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
        {
            CurrentState = Define.PlayerStatus.Running;
        }
            
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
        if(_target != null)
        {
            NormalMobStat targetStat = _target.GetComponent<NormalMobStat>();
            Managers.Sound.Play($"SE/Hit");

            bool targetNotDead = TargetNotDead;
            targetStat.OnAttacked(_stat.Atk, ref targetNotDead);
            TargetNotDead = targetNotDead;
            Camera.main.GetComponent<CameraController>().AtkSetting(TargetNotDead);
            Camera.main.GetComponent<CameraController>().CamShake(10f, 1f, 0.4f);

            Vector3 targetColl = _target.GetComponent<Collider>().transform.position;
            targetColl.z -= 0.5f;
            targetColl.y += 0.5f;

            if (_punchEffect == null)
                _punchEffect = Managers.Resource.Instantiate("Effect/PunchHitOrange");
            _punchEffect.transform.position = targetColl;
            _punchEffect.GetComponent<ParticleSystem>().Play();
        }

        if(_touchBlock)
            _touchBlock = false;
    }
    #endregion

}