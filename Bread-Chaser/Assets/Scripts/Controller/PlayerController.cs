using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //x좌표 이동범위
    private float       maxXwidth = 1.5f;
    //회피 소모 거리
    private float       sideMovesSpd = 2.0f;
    private bool        isMove = false;

    private float       originY = -0.09f;
    private float       gravity = -9.81f;
    private float       moveTimeY = 0.3f;
    private bool        isJump = true;

    [SerializeField]
    private float       moveSpeed = 20.0f;

    private float       deathY = -3f;
    private Rigidbody   rigid;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
        //rigid.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);

        if (transform.position.y < deathY)
        {
            Debug.Log("hp -1");
        }
    }

    public void MovetoX(int x)
    {
        if (isMove == false)
            return;

        if( gameObject.transform.position.x > maxXwidth || gameObject.transform.position.x < -maxXwidth)
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -maxXwidth, maxXwidth); // x 값 제한
            transform.position = pos;
        }
    }
}
