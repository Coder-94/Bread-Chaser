using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{

    #region variables

    protected enum AnimParameters
    {
        TriggerAtk,
        TriggerJump
    }

    public bool         _inputBlock = false;
    public bool         _isAtk = false;

    PlayerStat          _stat;
    Animator            _anim;
    protected int[]     _hashedParams;

    Vector3             _targetPos;
    int                 _targetMask = (1 << (int)Define.Layer.Enemy);
    bool                _alreadyLockedOn = false;

    float               _jumpForce = 3;

    #endregion

    #region unity scripts

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        Attack();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (_targetPos == null) return;

        _anim.SetLookAtWeight(1.0f);
        _anim.SetLookAtPosition(_targetPos);
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

        _anim = GetComponent<Animator>();
        _stat = GetComponent<PlayerStat>();

        _targetPos = Managers.Scene.CurrentScene.spawnedMobChecker[0].spawnedPos;
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
                Debug.Log("타깃 좌로 변경");
                break;
            case Define.TouchEvent.RightTap:
                Debug.Log("타깃 우로 변경");
                break;
            case Define.TouchEvent.HoldedFingerReleased:
                Debug.Log("스킬발동");
                break;
            case Define.TouchEvent.FingerReleased:
                Debug.Log("손가락 제거");
                break;
            case Define.TouchEvent.UpSwipe:
                Jump();
                break;
            case Define.TouchEvent.DownSwipe:
                Debug.Log("슬라이딩");
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
        Debug.Log("점프");
    }

    #endregion

    #region auto attack

    void Attack()
    {
        Targeting();

        if (_isAtk)
            return;

        float atkSpeed = 1 / _stat.AtkSpd;
        StartCoroutine(AttackCoroutine(atkSpeed));
    }

    void Targeting()
    {
        Debug.DrawRay(transform.position + Vector3.up, _targetPos.normalized, Color.green);
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up, _targetPos, out hit, 100.0f, _targetMask) && _alreadyLockedOn == false)
        {
            Debug.Log("Mob Detected!");

            GameObject target = hit.collider.gameObject;
            GameObject cursor = Managers.Resource.Instantiate("UI/Targeting");
            LockOnController cursorControl = cursor.GetComponent<LockOnController>();

            Vector3 pos = cursor.transform.position;

            pos.x = target.transform.position.x;
            pos.y = target.transform.position.y + 1.5f;
            pos.z = target.transform.position.z + 0.5f;

            cursorControl.pos = pos;
            cursor.transform.position = pos;

            _alreadyLockedOn = true;
        }
    }

    protected IEnumerator AttackCoroutine(float attackSpeed)
    {
        if (_isAtk)
            yield break;

        _isAtk = true;

        _anim.SetTrigger((_hashedParams[(int)AnimParameters.TriggerAtk]));

        yield return new WaitForSeconds(attackSpeed);

        _isAtk = false;
    }

    #endregion

    #endregion

    #region Anim Events

    void BooleanInit()
    {
        _inputBlock = false;
        _isAtk = false;
    }

    #endregion
    
}
