using UnityEngine;

public class CursorRotating : MonoBehaviour
{
    protected float _rotateSpeed = 270;

    protected  void Update()
    {
        transform.Rotate(new Vector3(0, 0, _rotateSpeed * Time.deltaTime));
    }
}
