using UnityEngine;

public abstract class BaseMobController : MonoBehaviour
{
    #region variables

    protected GameObject target;

    #endregion

    #region Start & Update

    void Start()
    {
        Init();
    }

    #endregion

    #region Initialize
    protected virtual void Init()
    {
        target = Managers.Scene.player;
    }
    #endregion

    protected virtual void OnUpdate(float distBtwnPlayer)
    {
        if (target != null)
        {
            Vector3 dir = target.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);
            
            gameObject.transform.position = new Vector3(gameObject.transform.position.x,
                                                        gameObject.transform.position.y,
                                                        target.transform.position.z + distBtwnPlayer);
        }
    }

    protected abstract void Clear();
}
