using System.Collections;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    #region variables
    protected float         _maxXwidth = 5.5f;
    [SerializeField]
    protected float         _sideMovesSpd = 15.0f;

    protected float         _originY = -0.09f;
    protected float         _jumpPower = 5f;
    protected bool          _isJump = false;

    [SerializeField]
    protected float         _moveSpeed = 20.0f;

    protected float         deathY = -3f;
    protected Rigidbody     rigid;
    protected Animator      anim;
    #endregion

    #region Start & Update
    protected virtual void Start()
    {
        rigid                       = GetComponent<Rigidbody>();
        anim                        = GetComponent<Animator>();

        Managers.Input.KeyAction    -= Move;
        Managers.Input.KeyAction    = Move;
    }

    protected virtual void Update()
    {
        transform.position += Vector3.forward * _moveSpeed * Time.deltaTime;
        //rigid.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
    }
    #endregion

    #region Jump
    protected void Jump()
    {
        if (_isJump == false)
        {
            rigid.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
            _isJump = true;
        }
    }

    //Landing
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
            _isJump = false;  
    }
    #endregion

    #region Move
    protected void Move()
    {
        if (gameObject.transform.position.x > _maxXwidth || gameObject.transform.position.x < -_maxXwidth)
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, -_maxXwidth, _maxXwidth);
            transform.position = pos;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * Time.deltaTime * _sideMovesSpd;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * Time.deltaTime * _sideMovesSpd;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }
    #endregion
}
