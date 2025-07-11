using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Define;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{

    #region variables

    protected enum AnimParameters
    {
        TriggerAtk,
        TriggerJump,
        TriggerBackStep,
        IsAtk
    }

    public Define.PlayerStatus CurrentStatus { get; protected set; }

    bool _inputBlock = false;
    bool _isAtk = false;
    PlayerStat _stat;
    Animator _anim;

    protected int[] _hashedParams;
    private Queue<Action> _movementQueue = new Queue<Action>();

    GameObject _target = null;
    int _enemyMask = (1 << (int)Define.Layer.Enemy);

    Vector3 _originPos;
    GameObject _cursor = null;
    Rigidbody _rb;



    #endregion

    #region unity scripts

    private void Start()
    {
        Init();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (_target == null) return;

        _anim.SetLookAtWeight(1.0f);
        _anim.SetLookAtPosition(_target.transform.position);
    }

    private void OnCollisionEnter(Collision collision)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == (int)Define.Layer.Obstacle)
            Debug.Log("Obstacle Collisioned");
    }

    private void FixedUpdate()
    {
        RbControl();
    }

    private void Update()
    {
        Debug.Log($"CurrentStatus: {CurrentStatus}");
    }
    #endregion

    #region initialize

    void Init()
    {
        Managers.Input.TouchAction -= PlayerActor;
        Managers.Input.TouchAction += PlayerActor;

        int animParamLength = System.Enum.GetValues(typeof(AnimParameters)).Length;
        _hashedParams = new int[animParamLength];

        for (int i = 0; i < animParamLength; i++)
        {
            AnimParameters param = (AnimParameters)i;
            string key = param.ToString();
            _hashedParams[i] = Animator.StringToHash(key);
        }

        CurrentStatus = Define.PlayerStatus.Running;
        _anim = GetComponent<Animator>();
        _stat = GetComponent<PlayerStat>();
        _rb = GetComponent<Rigidbody>();

    }
    #endregion

    #region player movement

    void PlayerActor(Define.TouchEvent evt)
    {
        if (_inputBlock)
            return;

        switch (evt)
        {
            case Define.TouchEvent.Tap:
                Debug.Log("ÅÇ");
                break;
            case Define.TouchEvent.HoldedFingerReleased:
                break;
            case Define.TouchEvent.Holding:
                CurrentStatus = Define.PlayerStatus.LockOning;
                break;
            case Define.TouchEvent.UpSwipe:
                Jump();
                break;
            case Define.TouchEvent.DownSwipe:
                CurrentStatus = Define.PlayerStatus.BackStepping;
                break;
            case Define.TouchEvent.LeftSwipe:
                Debug.Log("ÁÂ·Î ÀÌµ¿");
                break;
            case Define.TouchEvent.RightSwipe:
                Debug.Log("¿ì·Î ÀÌµ¿");
                break;
        }
    }

    void PlayerControl() 
    {
        switch (CurrentStatus)
        {
            case Define.PlayerStatus.Running:
                break;
            case Define.PlayerStatus.Attacking:
                break;
            case Define.PlayerStatus.BackStepping:
                BackStep();
                break;
            case Define.PlayerStatus.LockOning:
                LockOn();
                break;
            case Define.PlayerStatus.Jumping:
                break;
        }
    }

    void RbControl()
    {
        switch (CurrentStatus)
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

    void Jump()
    {
        if (CurrentStatus == Define.PlayerStatus.Running)
            StartCoroutine(JumpCoroutine(1.5f));
    }

    IEnumerator JumpCoroutine(float cooldownTime)
    {
        CurrentStatus = Define.PlayerStatus.Jumping;
        _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerJump]);

        int random = UnityEngine.Random.Range(1, 4);

        Managers.Sound.Play($"SE/JumpVoice{random}");
        _movementQueue.Enqueue(Jumper);

        yield return new WaitForSeconds(cooldownTime);
    }

    void Jumper()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
    }

    #endregion

    #region targetting

    void LockOn()
    {

    }

    #endregion

    #region atk


    void Attack()
    {
        if (!_target)
            return;

        if (CurrentStatus == Define.PlayerStatus.Running)
        {
            _originPos = gameObject.transform.position;
            _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerAtk]);
            CurrentStatus = Define.PlayerStatus.Attacking;
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

    void BackStep()
    {
        if (CurrentStatus != Define.PlayerStatus.Attacking)
            return;

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
            CurrentStatus = Define.PlayerStatus.Running;
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
        CurrentStatus = Define.PlayerStatus.Running;
    }

    #endregion

}