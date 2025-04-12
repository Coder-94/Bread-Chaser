using UnityEngine;

public class ObjectRotator : ObjectFloater
{
    protected float _rotateSpeed = 270;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        transform.Rotate(new Vector3(0, 0, _rotateSpeed * Time.deltaTime));
    }
}
