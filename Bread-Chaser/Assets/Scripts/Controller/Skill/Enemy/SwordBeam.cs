using UnityEngine;

public class SwordBeam : MonoBehaviour
{
    private float moveSpeed = 70f;
    private float damage = 5f; 
    private Vector3 moveDirection = Vector3.forward;

    private void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerStat>()?.OnPlAttacked(gameObject, damage);
        }
    }
}
