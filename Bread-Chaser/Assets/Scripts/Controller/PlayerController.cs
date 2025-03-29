using System.Collections;
using UnityEngine;

public class PlayerController : PlayerMover
{


    private void Update()
    {
        transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
        //rigid.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);

        if (transform.position.y < deathY)
        {
            Debug.Log("hp -1");
        }
    }

}
