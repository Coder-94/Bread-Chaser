using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class PlayerController : PlayerMover
{
    #region variables
    int _layerMaskF = (1 << (int)Define.Layer.Obstacle)
                   | (1 << (int)Define.Layer.SlideObstacle)
                   | (1 << (int)Define.Layer.CanParryAtk)
                   | (1 << (int)Define.Layer.CantParryAtk);

    int _layerMask = (1 << (int)Define.Layer.Wall);
    #endregion

    #region Start & Update
    protected override void Start()
    {
        base.Start();

    }

    protected override void Update()
    {
        base.Update();

        if (transform.position.y <= deathY)
            Debug.Log("hp -1");

        ObjectChecker();

    }
    #endregion

    void ObjectChecker()
    {
        #region local variables
        float           rayDist = 10f;
        float           f_rayDist = 10f;
        Vector3         startPoint = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        Vector3         forwardDir = transform.TransformDirection(Vector3.forward);
        Vector3         rightDir = transform.TransformDirection(Vector3.right);
        Vector3         leftDir = transform.TransformDirection(Vector3.left);

        Debug.DrawRay   (startPoint, forwardDir * f_rayDist, Color.red);
        Debug.DrawRay   (startPoint, rightDir * rayDist, Color.blue);  
        Debug.DrawRay   (startPoint, leftDir * rayDist, Color.blue);
        #endregion

        if (Physics.Raycast(startPoint, forwardDir, out RaycastHit hit, 3f, _layerMaskF))
        {
            //장애물 및 공격 감지
            Debug.Log("몬가있음;");
        }
        if (Physics.Raycast(startPoint, rightDir, out RaycastHit hitL, rayDist) || Physics.Raycast(startPoint, leftDir, out RaycastHit hitR, rayDist, _layerMask))
        {
            //벽 감지
        }

    }
}
