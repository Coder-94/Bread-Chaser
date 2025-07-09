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

    public Define.PlayerStatus  CurrentStatus { get; protected set; }

    bool                        _inputBlock = false;
    bool                        _isAtk = false;
    PlayerStat                  _stat;
    Animator                    _anim;
    
    protected int[]             _hashedParams;
    private Queue<Action>       _movementQueue = new Queue<Action>();

    GameObject                  _target = null;
    int                         _enemyMask = (1 << (int)Define.Layer.Enemy);

    Vector3                     _originPos;
    GameObject                  _cursor= null;

    

    
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == (int)Define.Layer.Obstacle)
            Debug.Log("Obstacle Collisioned");
    }

    private void FixedUpdate()
    {
        if(CurrentStatus == Define.PlayerStatus.Attacking)
        {
            if (_target)
            {
                Vector3 dir = _target.transform.position - transform.position;
                dir.z -= 1.0f;
                dir.y = 0;
                float moveDist = Mathf.Clamp(15f * Time.deltaTime, 0, dir.magnitude);
                transform.position += dir.normalized * moveDist;
            }
        }
        else if (CurrentStatus == Define.PlayerStatus.BackStepping)
        {
            Vector3 dir = _originPos - transform.position;
            float moveDist = Mathf.Clamp(15f * Time.deltaTime, 0, dir.magnitude);
            transform.position += dir.normalized * moveDist;
        }

        while(_movementQueue.Count > 0 )
        {
            Action action = _movementQueue.Dequeue();
            action?.Invoke();
        }
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
        Managers.Input.HoldedTimeAction -= Attack;
        Managers.Input.HoldedTimeAction += Attack;

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


    }
    #endregion

    #region player movement

    void PlayerActor(Define.TouchEvent evt)
    {
        if (_inputBlock)
            return;

        switch(evt)
        {
            case Define.TouchEvent.Tap:
                Debug.Log("탭");
                break;
            case Define.TouchEvent.LeftTap:
                
                break;
            case Define.TouchEvent.RightTap:
                
                break;
            case Define.TouchEvent.HoldedFingerReleased:
                break;
            case Define.TouchEvent.FingerReleased:
                Debug.Log("손가락 제거");
                break;
            case Define.TouchEvent.UpSwipe:
                Jump();
                break;
            case Define.TouchEvent.DownSwipe:
                BackStep();
                break;
            case Define.TouchEvent.LeftSwipe:
                Debug.Log("좌로 이동");
                break;
            case Define.TouchEvent.RightSwipe:
                Debug.Log("우로 이동");
                break;
        }
    }

    #region jump

    void Jump()
    {
        if (CurrentStatus == Define.PlayerStatus.Jumping)
            return;

        StartCoroutine(JumpCoroutine(1.5f));
    }

    IEnumerator JumpCoroutine(float cooldownTime)
    {
        Debug.Log("점프");
        CurrentStatus = Define.PlayerStatus.Jumping;
        _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerJump]);

        int random = UnityEngine.Random.Range(1, 4);

        Managers.Sound.Play($"SE/JumpVoice{random}");
        _movementQueue.Enqueue(Jumper);

        yield return new WaitForSeconds(cooldownTime);

        CurrentStatus = Define.PlayerStatus.Running;
    }

    void Jumper()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
    }

    #endregion

    /// 자동락온좆까

    #region atk


    void Attack(float holdedTime)
    {
        if (!_target)
            return;

        if(CurrentStatus == Define.PlayerStatus.Running)
        {
            if (Time.time < holdedTime + 1f)
            {
                Debug.Log("평타!");
                _originPos = gameObject.transform.position;
                _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerAtk]);
                CurrentStatus = Define.PlayerStatus.Attacking;
            }
            else
            {
                Debug.Log("스킬발동!");
            }
        }
            
    }

    void BackStep()
    {
        if (CurrentStatus != Define.PlayerStatus.Attacking)
            return;

        Debug.Log("백스탭!");

        _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerBackStep]);
        CurrentStatus = Define.PlayerStatus.BackStepping;
    }

    #endregion

    #endregion

    #region Anim Events

    public void OnAttack()
    {
        NormalMobStat targetStat = _target.GetComponent<NormalMobStat>();
        Managers.Sound.Play($"SE/Hit");
        targetStat.OnAttacked(_stat.Atk);
        if (targetStat.Hp <= 0 || !_target)
        {
            _target = null;
            BackStep();
        }
        Debug.Log(_target);
    }

    public void StatusInit()
    {
        CurrentStatus = Define.PlayerStatus.Running;
    }

    #endregion
    
}
