using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BulletController : MonoBehaviour
{
    float _bulletSpeed = 15f;
    float _spawnTime;

    GameObject _target;
    public NormalMobStat parentStat;

    #region Unity Scripts
    private void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        _spawnTime = 0f;
    }

    void Init()
    {
        _target = Managers.Game.GetPlayer();
        gameObject.transform.parent = null;

        /*if (_target != null)
        {
            Vector3 dir = (_target.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(dir);
        }*/
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
                /* Test targetStat = target.GetComponent<Test>();
                 targetStat.TestDamage(parentStat);*/

                Managers.Resource.Destroy(gameObject);
            }
        }
    }
    #endregion

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
