using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BulletController : MonoBehaviour
{
    float _bulletSpeed = 20f;
    float _spawnTime;

    GameObject _target;
    GameObject _shooter;

    #region Unity Scripts
    private void OnEnable()
    {
        Init();
        _spawnTime = 0f;
    }

    void Init()
    {
        _target = Managers.Game.GetPlayer();
        gameObject.transform.parent = null;

    }

    private void Update()
    {
        Vector3 dir = Vector3.forward * _bulletSpeed * Time.deltaTime;

        transform.Translate(dir, Space.Self);

        LifeTimeChecker();  
    }

    private void OnDisable()
    {
        transform.rotation = Quaternion.identity;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject == _target)
        {
            if (_target != null)
            {
                PlayerStat targetStat = _target.GetComponent<PlayerStat>();
                PlayerController control = _target.GetComponent<PlayerController>();

                if(control.CurrentState != Define.PlayerStatus.Attack)
                    targetStat.OnPlAttacked(_shooter);

                Managers.Resource.Destroy(gameObject);
            }
        }
    }
    #endregion

    public void SetShooter(GameObject shooter)
    {
        _shooter = shooter;
        if (transform.parent != null)
        {
            transform.parent = null;
        }
    }

    #region LifeTime Controller
    private void LifeTimeChecker()
    {
        _spawnTime += Time.deltaTime;
        if (_spawnTime >= 2)
        {
            _spawnTime = 0f;
            Managers.Resource.Destroy(gameObject);
        }
    }
    #endregion
}
