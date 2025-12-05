using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    protected float _rotateSpeed = 270;

    protected void Update()
    {
        transform.Rotate(new Vector3(_rotateSpeed * Time.deltaTime, 0, 0));
    }
}
