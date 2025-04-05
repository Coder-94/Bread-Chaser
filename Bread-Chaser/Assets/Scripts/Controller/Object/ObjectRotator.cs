using UnityEngine;

public class ObjectRotator : ObjectFloater
{
    protected float _rotateSpeed;

    protected override void Start()
    {
        base.Start();
        _rotateSpeed = Random.Range(180, 360);
    }

    protected override void Update()
    {
        base.Update();
        transform.Rotate(new Vector3(0, 0, _rotateSpeed * Time.deltaTime));
    }
}
