using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    public Define.Scene     SceneType { get; protected set; } = Define.Scene.Unknown;
    

    void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        Object obj = GameObject.FindAnyObjectByType(typeof(EventSystem));
        if (obj == null)
            Managers.Resource.Instantiate("Prefabs/UI/EventSystem").name = "@EventSystem";
        Managers.Scene.player = GameObject.FindGameObjectWithTag("Player");
    }

    public abstract void Clear();
}
