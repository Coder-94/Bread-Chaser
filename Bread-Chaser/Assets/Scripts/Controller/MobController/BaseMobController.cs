using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public abstract class BaseMobController : MonoBehaviour
{
    #region variables

    protected GameObject    player;
    public Vector3          initPos;
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
        player =    Managers.Scene.CurrentScene.Player;
    }
    #endregion

    #region TransformFixer

    protected void RotFixer(GameObject target = null)
    {
        Vector3 dir;

        if (target != null)
        {
            dir = target.transform.position - transform.position;
        }            
        else
            dir = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
        Quaternion quat = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
    }

    protected void PosFixer()
    {
        Define.PlayerStatus currentStatus = player.GetComponent<PlayerController>().CurrentState;
        if(currentStatus!= Define.PlayerStatus.Attacking && currentStatus != Define.PlayerStatus.BackStepping) 
        {
            Vector3 newPosition = transform.position;

            newPosition.x = player.transform.position.x + initPos.x;

            transform.position = newPosition;
        }
    }

    #endregion

    protected abstract void Clear();
    protected abstract void Attack();
    protected abstract void LocalAtk();
    protected abstract void Death();
}
