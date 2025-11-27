using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    #region variables

    protected enum AnimParameters
    {
        TriggerRoundAtk,
        TriggerAtk,
        TriggerLeftAtk,
        TriggerRightAtk,
        TriggerJump,
        TriggerBackStep,
        TriggerLeftSlide,
        TriggerRightSlide,
        TriggerSkill,
        TriggerPassive,
        TriggerDMG,
        IsAtk
    }


    public bool TargetNotDead { get; protected set; } = false;
    public Vector3 OriginPos { get; protected set; } = new Vector3(9999, 9999, 9999);

    protected Renderer[]            _renderers;
    protected Define.PlayerStatus   _state;
    protected Define.PLRailPos      _railPos;
    protected bool                  _isJumping = false;
    protected PlayerStat            _stat;
    protected Animator              _anim;

    protected bool                  _touchBlock = false;
    protected int[]                 _hashedParams;
    protected Queue<Action>         _movementQueue = new Queue<Action>();
    protected CameraController      _camController;

    protected GameObject            _target = null;
    protected int                   _enemyMask = (1 << (int)Define.Layer.Enemy);
    
    protected GameObject            _targeting = null;
    protected Rigidbody             _rb;

    protected GameObject            _dashEffect;
    protected GameObject            _punchEffect;

    protected GameObject            _shieldEffect;
    protected GameObject            _currentShieldHealth;

    public Define.IncreaseAbleStat EvolvedType; /*{ get; protected set; }*/
    public bool                     IsSkillCool { get; protected set; } = false;
    public float                    CoolTime { get; protected set; } = 0;
    #endregion

    public void SetCool(float cool) {  CoolTime = cool; }

    #region currentState
    public Define.PlayerStatus CurrentState
    {
        get { return _state; }
        set
        {
            if (_state == value) return;

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
                    {
                        _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerBackStep]);
                        BackStep();
                    }
                    break;
                case Define.PlayerStatus.LeftMoving:
                    _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerLeftSlide]);
                    break;
                case Define.PlayerStatus.RightMoving:
                    _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerRightSlide]);
                    break;
                case Define.PlayerStatus.Damaged:
                    Damaged();
                    break;
                case Define.PlayerStatus.Channeling:
                    _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerSkill]);
                    break;
            }
        }
    }

    #region functionCR

    protected void Damaged()
    {

        _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerDMG]);
        Managers.Sound.Play("SE/Hit");

        if (OriginPos != new Vector3(9999, 9999, 9999))
        {
            BackStep();
        }
        else
        {
            _touchBlock = false;
            StartCoroutine(RecoverFromDamage());
        }
    }

    private IEnumerator RecoverFromDamage()
    {
        yield return new WaitForSeconds(0.2f);
        CurrentState = Define.PlayerStatus.Running;
    }

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
            if (OriginPos == new Vector3(9999, 9999, 9999))
                OriginPos = gameObject.transform.position;
            else
                Debug.Log($"[Error] OriginPos is {OriginPos}");

            if (_stat.EvolutionData[Define.IncreaseAbleStat.SkillDMG].FirstEvolve)
                _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerRoundAtk]);
            else
                _anim.SetTrigger(_hashedParams[(int)AnimParameters.TriggerAtk]);
            _dashEffect.GetComponent<ParticleSystem>().Play();
        }
    }

    protected void BackStep()
    {
        _target.GetComponent<BaseMobController>().TargetCheck(false);
        TargetNotDead = false;
        _touchBlock = false;
        _target = null;
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
        _dashEffect = Util.FindChild(gameObject, "DashSmoke");
        _railPos = Define.PLRailPos.SecondRail;

        _camController = Camera.main.GetComponent<CameraController>();

        if (_stat != null)
        {
            _stat.OnFirstEvolved += HandleFirstEvolution;
            _stat.OnSecondEvolved += HandleSecondEvolution;
        }
        _renderers = GetComponentsInChildren<Renderer>();
    }

    private void HandleFirstEvolution(Define.IncreaseAbleStat stat)
    {
        if (stat == Define.IncreaseAbleStat.SkillDMG)
        {
            PunchEffectNull();
        }
    }

    private void HandleSecondEvolution(Define.IncreaseAbleStat stat, float coolTime)
    {
        SetCool(coolTime);
        EvolvedType = stat;
        GameObject.Find("SkillBtn").GetComponent<SkillBtn>().SkillInit(EvolvedType);
    }


    public void StartBlinkEffect(float duration)
    {
        if (_renderers == null || _renderers.Length == 0) return;

        StartCoroutine(BlinkCoroutine(duration));
    }

    protected IEnumerator BlinkCoroutine(float duration)
    {
        float timer = 0f;
        float blinkSpeed = 0.1f;

        while (timer < duration)
        {
            foreach (Renderer renderer in _renderers)
            {
                if (renderer != null)
                    renderer.enabled = !renderer.enabled;
            }

            yield return new WaitForSeconds(blinkSpeed);
            timer += blinkSpeed;
        }

        foreach (Renderer renderer in _renderers)
        {
            if (renderer != null)
                renderer.enabled = true;
        }
    }
    #endregion

    #region unity scripts

    private void Start()
    {
        Init();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (_target == null)
        {
            _anim.SetLookAtWeight(0f);
            return;
        }

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

    protected virtual void PunchEffectNull() { }
    protected virtual void JumpRB() { }
    protected virtual void RbControl() { }
    protected virtual void PlayerControl(Define.TouchEvent evt) { }
    protected virtual void PlayerActor() { }
}
