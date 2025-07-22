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

    public bool TargetNotDead { get; protected set; } = false;

    protected Define.PlayerStatus _state;

    protected bool _isJumping = false;
    protected PlayerStat _stat;
    protected Animator _anim;

    protected int[] _hashedParams;
    protected Queue<Action> _movementQueue = new Queue<Action>();

    protected GameObject _target = null;
    protected int _enemyMask = (1 << (int)Define.Layer.Enemy);

    protected Vector3 _originPos;
    protected GameObject _targeting = null;
    protected Rigidbody _rb;

    protected GameObject _punchEffect;
    protected GameObject _dashEffect;
    #endregion

    #region currentState
    public Define.PlayerStatus CurrentState
    {
        get { return _state; }
        set
        {
            _state = value;

            switch (_state)
            {
                case Define.PlayerStatus.Running:
                    break;
                case Define.PlayerStatus.Jumping:
                    Jump();
                    break;
                case Define.PlayerStatus.LockOning:
                    StartLockOn();
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

    #region functionCR
    protected void Jump()
    {
        if (!_isJumping)
        {
            _isJumping = true;
            _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerJump]);

            int random = UnityEngine.Random.Range(1, 4);

            Managers.Sound.Play($"SE/JumpVoice{random}");
            _movementQueue.Enqueue(JumpRB);
        }
    }
    protected void Attack()
    {
        if (_target != null)
        {
            _originPos = gameObject.transform.position;
            _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerAtk]);
            _dashEffect.GetComponent<ParticleSystem>().Play();
        }
    }

    protected void BackStep()
    {
        Debug.Log("¹é½ºÅÇ!");

        _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerBackStep]);
    }

    protected void StartLockOn()
    {
        if (_targeting == null)
            _targeting = Managers.UI.ShowPopUpUI<LockOn>().gameObject;

        Touch touch = Input.GetTouch(0);
        Vector2 currentTouchPos = touch.position;

        _targeting.GetComponent<LockOn>().SetFirstTouch(currentTouchPos);
    }
    #endregion

    #endregion

    #region initialize

    void Init()
    {
        Managers.Input.TouchAction -= PlayerControl;
        Managers.Input.TouchAction += PlayerControl;

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
        _punchEffect = Util.FindChild(gameObject, "PunchHitBlue", true);
        _dashEffect = Util.FindChild(gameObject, "BlueDash");
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
        if(CurrentState == Define.PlayerStatus.Jumping &&
            collision.collider.gameObject.layer == (int)Define.Layer.Ground)
        {
            _isJumping = false;
            CurrentState = Define.PlayerStatus.Running;
        }
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
        PlayerActor();
    }
    #endregion

    protected virtual void JumpRB() { }
    protected virtual void RbControl() { }
    protected virtual void PlayerControl(Define.TouchEvent evt) { }
    protected virtual void PlayerActor() { }
}
