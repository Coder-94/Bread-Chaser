using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public abstract class BaseMobController : MonoBehaviour
{
    #region variables
    protected enum AnimParameters
    {
        IsDead,
        TriggerStun,
        IsCasting,
        TriggerAtk,
        TriggerSpAtk,
        TriggerEncountLocalAtk,
        TriggerLocalAtk
    }

    protected Animator          anim;
    protected GameObject        player;
    protected PlayerController  plController;
    public Vector3              initPos;
    public GameObject           targetedPos;
    protected PlayerStat        plStat;
    public bool                 ImTargeted { get; protected set; } = false;
    #endregion

    #region Start & Update

    void Start()
    {
        Init();
    }

    #endregion

    #region Init
    protected virtual void Init()
    {
        anim = GetComponent<Animator>();
        player = Managers.Game.GetPlayer();
        plController = player.GetComponent<PlayerController>();
        targetedPos = transform.GetChild(transform.childCount - 1).gameObject;
        plStat = player.GetComponent<PlayerStat>();
    }
    #endregion

    #region TransformFixer

    protected void RotFixer(GameObject target = null)
    {
        Vector3 dir;

        if (plController.CurrentState != Define.PlayerStatus.Jumping)
        {
            if (target != null)
            {
                dir = target.transform.position - transform.position;
            }
            else
                dir = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
        }
    }

    protected virtual void PosFixer()
    {
        Define.PlayerStatus currentStatus = player.GetComponent<PlayerController>().CurrentState;
        if (currentStatus!= Define.PlayerStatus.Attacking && currentStatus != Define.PlayerStatus.Attack && currentStatus != Define.PlayerStatus.BackStepping) 
        {
            Vector3 newPosition = transform.position;

            newPosition.x = player.transform.position.x + initPos.x;

            transform.position = newPosition;
        }
    }

    #endregion


    public void TargetCheck(bool mystate) { ImTargeted = mystate; }

    protected abstract void Clear();
    protected abstract void Attack();
    protected abstract void SPAtk();
    protected abstract void LocalAtk();
}
