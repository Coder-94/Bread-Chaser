using UnityEngine;

public class LockOnController : MonoBehaviour
{
    GameObject  _target;
    Vector3     _pos;
    float       _rotateSpeed = 270;
    public void Init(GameObject target, Vector3 pos)
    {
        _target = target;
        _pos = pos;
    }


    void Update()
    {
        transform.Rotate(new Vector3(0, _rotateSpeed * Time.deltaTime, 0));

        float targetHp = _target.GetComponent<NormalMobStat>().Hp;

        if(targetHp <= 0)
        {
            _target = null;
            _pos = new Vector3(0,0,0);

            Managers.Resource.Destroy(gameObject);
        }
    }
}
