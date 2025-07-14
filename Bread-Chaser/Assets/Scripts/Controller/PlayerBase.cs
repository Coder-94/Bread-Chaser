using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    #region variables

    protected enum AnimParameters
    {
        TriggerAtk,
        TriggerJump,
        TriggerBackStep,
        IsAtk
    }

    public Define.PlayerStatus CurrentState
    {
        get { return _state; }
        set
        {
            _state = value;

            Animator anim = GetComponent<Animator>();
            switch (_state)
            {
                case Define.PlayerStatus.Running:
                    break;
                case Define.PlayerStatus.Jumping:
                    Jump();
                    break;
                case Define.PlayerStatus.LockOning:
                    break;
                case Define.PlayerStatus.Attacking:
                    Attack();
                    break;
                case Define.PlayerStatus.BackStepping:
                    BackStep();
                    break;
            }
        }
    }

    protected Define.PlayerStatus _state;
    protected bool _inputBlock = false;
    protected bool _isAtk = false;
    protected PlayerStat _stat;
    protected Animator _anim;

    protected int[] _hashedParams;
    protected Queue<Action> _movementQueue = new Queue<Action>();

    protected GameObject _target = null;
    protected int _enemyMask = (1 << (int)Define.Layer.Enemy);

    protected Vector3 _originPos;
    protected GameObject _cursor = null;
    protected Rigidbody _rb;

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

        CurrentState = Define.PlayerStatus.Running;
        _anim = GetComponent<Animator>();
        _stat = GetComponent<PlayerStat>();
        _rb = GetComponent<Rigidbody>();

    }
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
        Debug.Log($"CurrentStatus: {CurrentState}");
    }
    #endregion

    protected virtual void Attack() { }
    protected virtual void BackStep() { }
    protected virtual void Jump() { }
    protected virtual void RbControl() { }
    protected virtual void PlayerActor(Define.TouchEvent evt) { }
    protected virtual void PlayerControl() { }
}
