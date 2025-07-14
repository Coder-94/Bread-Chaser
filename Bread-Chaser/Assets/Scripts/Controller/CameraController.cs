using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject          _target;
    Define.PlayerStatus _status;
    void Start()
    {
        Init();
    }

    void Init()
    {
        _target = Managers.Scene.CurrentScene.Player;
        _status = _target.GetComponent<PlayerController>().CurrentState;
    }

    void Update()
    {
        CameraControl();
    }

    void CameraControl()
    {
        switch (_status)
        {
            case Define.PlayerStatus.Running:
                RotFixer(15.83f);
                PosFixer(-1.7f, 2.445f, -2.27f);
                break;
            case Define.PlayerStatus.Attacking:
                break;
        }
    }

    void RotFixer(float x=0, float y=0, float z=0)
    {
        transform.rotation = Quaternion.identity;
        transform.rotation = Quaternion.Euler(x, y, z);
    }

    void PosFixer(float x, float y, float z)
    {
        transform.position = new Vector3(x, y, z);
    }
}
