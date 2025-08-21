using UnityEngine;
using static UnityEngine.UI.Image;

public class LockOn: UIPopUp
{
    const float             MAXDISTANCE = 100f;
    private GameObject      _target = null;
    private RectTransform   _myPos;

    protected Vector2 _lastTouchPos;
    protected bool _isTouching = false;

    protected GameObject _cursor = null;
    protected GameObject _onTargetcursor = null;
    protected int _enemyMask = (1 << (int)Define.Layer.Enemy);

    protected float _cameraRotCorrection = 3f;
    enum GameObjects
    {
        LockOn,
        TargetCursor,
        OnTargetCursor
    }

    void Update()
    {
        Targeting();
    }

    public override void Init()
    {
        base.Init();

        _myPos = GetComponent<RectTransform>();
        Bind<GameObject>(typeof(GameObjects));
        Managers.Input.TouchAction -= LockOnMoving;
        Managers.Input.TouchAction += LockOnMoving;

        _cursor = GetObject((int)GameObjects.TargetCursor);
        _onTargetcursor = GetObject((int)GameObjects.OnTargetCursor);
    }

    void Targeting()
    {
        if (_cursor != null)
        {
            RectTransform rt = _cursor.GetComponent<RectTransform>();

            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, rt.position);
            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _enemyMask))
            {
                _target = hit.collider.gameObject;
            }

            if (_target != null)
            {
                RectTransform onTargetRect = _onTargetcursor.GetComponent<RectTransform>();
                Vector3 targetPos = _target.GetComponent<Collider>().bounds.center;

                Vector3 onTargetScreenPos = Camera.main.WorldToScreenPoint(targetPos);
                onTargetRect.position = onTargetScreenPos;
            }
        }
    }

    void LockOnMoving(Define.TouchEvent evt)
    {
        if (_cursor == null)
            return;

        if (evt == Define.TouchEvent.Holding)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 currentTouchPos = touch.position;

                if (_isTouching)
                {
                    Vector2 delta = currentTouchPos - _lastTouchPos;
                    _lastTouchPos = currentTouchPos;

                    RectTransform cursorRect = _cursor.GetComponent<RectTransform>();
                    cursorRect.anchoredPosition += delta * 3.5f;
                }
            }
        }
        else if(evt == Define.TouchEvent.HoldedFingerReleased)
        {
            _isTouching = false;
            ClosePopUpUI();
        }

    }

    public GameObject GetTarget()
    {
        if(_target != null)
        {
            _target.GetComponent<BaseMobController>().TargetCheck(true);
            return _target;
        }
        return null;
    }

    public void SetFirstTouch(Vector2 currentTouchPos)
    {
        _lastTouchPos = currentTouchPos;
        _isTouching = true;
    }
}
