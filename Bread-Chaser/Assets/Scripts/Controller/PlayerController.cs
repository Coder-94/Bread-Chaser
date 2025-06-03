using UnityEngine;

public class PlayerController : MonoBehaviour
{
    protected enum AnimParameters
    {
        TriggerAtk
    }

    PlayerStat          _stat;
    Animator            _anim;
    protected int[]     _hashedParams;

    private void Start()
    {
        Init();
        _anim = GetComponent<Animator>();
    }

    void Init()
    {
        Managers.Input.KeyAction -= PlayerActor;
        Managers.Input.KeyAction += PlayerActor;

        int animParamLength = System.Enum.GetValues(typeof(AnimParameters)).Length;
        _hashedParams = new int[animParamLength];

        for (int i = 0; i < animParamLength; i++)
        {
            AnimParameters param = (AnimParameters)i;
            string key = param.ToString();
            _hashedParams[i] = Animator.StringToHash(key);
        }

        _stat = GetComponent<PlayerStat>();
    }

    void PlayerActor()
    {
        //Atk
        if (Input.GetTouch(0).phase == TouchPhase.Began && _stat.isAtk == false)
        {
            _anim.SetTrigger((_hashedParams[(int)AnimParameters.TriggerAtk]));
            _stat.isAtk = true;
        }
    }

    private void Update()
    {

    }
}
