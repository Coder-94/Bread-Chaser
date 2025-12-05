using UnityEngine;

public class lighning : MonoBehaviour
{
    GameObject parent;

    private void OnEnable()
    {
        Init();
    }

    void Init()
    {
        parent = transform.root.GetComponent<ForestSkill>().parent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == (int)Define.Layer.Player)
        {
            if (parent != null)
                other.GetComponent<PlayerStat>().OnPlAttacked(parent, 1.2f);

        }
    }
}
