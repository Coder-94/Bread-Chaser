using UnityEngine;
using static UnityEngine.UI.Image;

public class LockOn: UIPopUp
{
    const float             MAXDISTANCE = 100f;
    private int             _enemyMask = (1 << (int)Define.Layer.Enemy);
    private GameObject      _target = null;
    private RectTransform   _myPos;

    enum GameObjects
    {
        LockOn,
        TargetCursor,
        OnTargetCursor
    }

    public override void Init()
    {
        base.Init();

        _myPos = GetComponent<RectTransform>();
        Bind<GameObject>(typeof(GameObjects));
        Managers.Input.TouchAction -= LockOnMoving;
        Managers.Input.TouchAction += LockOnMoving;
    }

    void Update()
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, _myPos.position);


        Ray ray = Camera.main.ScreenPointToRay(screenPos);


        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, MAXDISTANCE, _enemyMask))
        {
            _target = hit.collider.gameObject;
        }
    }

    void LockOnMoving(Define.TouchEvent evt)
    {

    }

    public GameObject SetTarget(GameObject target)
    {
        if(_target != null)
        {
            target = _target;

            return target;
        }

        return null;

    }

    public GameObject SetCursor()
    {
        GameObject cursor = GetObject((int)GameObjects.TargetCursor);
        Debug.Log(cursor);

        if (cursor != null)
            return cursor;
        
        return null;
    }
}
