using System.Collections;
using UnityEngine;

public class LocalAtkController : MonoBehaviour
{
    private GameObject  _owner;

    private Collider    _hitbox; // 공격 범위를 담당할 콜라이더

    private float       _attackDelay = 0.1f;
    private float       _attackDuration = 0.1f;
    private float       _duration = 1f;
    public void SetOwner(GameObject owner)
    {
        _owner = owner;
    }

    private void Awake()
    {
        _hitbox = GetComponent<SphereCollider>();        
    }

    private void OnEnable()
    {
        if (_hitbox != null)
            _hitbox.enabled = false;

        if (_owner == null)
        {
            Stat ownerStat = GetComponentInParent<Stat>();
            if (ownerStat != null)
            {
                _owner = ownerStat.gameObject;
            }
        }
        StartCoroutine(DespawnTimer());
        StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        yield return new WaitForSeconds(_attackDelay);

        if (_hitbox != null)
            _hitbox.enabled = true;

        yield return new WaitForSeconds(_attackDuration);

        if (_hitbox != null)
            _hitbox.enabled = false;

        float remainTime = _duration - (_attackDelay + _attackDuration);
        if (remainTime > 0)
            yield return new WaitForSeconds(remainTime);

        _owner = null;
        Managers.Resource.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == (int)Define.Layer.Player)
        {
            PlayerStat plStat = other.GetComponent<PlayerStat>();

            if (plStat != null)
            {
                Debug.Log("근접 공격 적중! (Trigger 방식)");
                plStat.OnPlAttacked(_owner);

                if (_hitbox != null) _hitbox.enabled = false;
            }
        }
    }

    protected IEnumerator DespawnTimer()
    {
        yield return new WaitForSeconds(_duration);

        Managers.Resource.Destroy(gameObject);
    }
}
