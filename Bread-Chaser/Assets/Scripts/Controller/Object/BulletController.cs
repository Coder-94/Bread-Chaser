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
        _target = Managers.Scene.CurrentScene.Player;
        _spawnTime = 0f;
        gameObject.transform.parent = null;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * _bulletSpeed * Time.deltaTime, Space.Self);

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

                Debug.Log("Attacked!");
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
