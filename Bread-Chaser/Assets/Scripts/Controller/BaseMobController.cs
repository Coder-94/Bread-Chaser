using UnityEngine;

public abstract class BaseMobController : MonoBehaviour
{
    #region variables

    protected GameObject player;

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
        player = Managers.Scene.CurrentScene.Player;
    }
    #endregion

    #region TransformFixer

    protected void RotFixer(GameObject target = null)
    {
        Vector3 dir;

        if (target != null)
            dir = target.transform.position - transform.position;
        else
            dir = new Vector3(transform.rotation.x, 180f, transform.rotation.z);

        Quaternion quat = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
    }
    //각도랑 위치 조정 분리했으니 공격패턴이랑 연동시켜야함
    protected void PosFixer(Vector3 pos)
    {
        gameObject.transform.position = pos;
    }

    #endregion

    
    protected abstract void Clear();
    protected abstract void StateChecker();
}
