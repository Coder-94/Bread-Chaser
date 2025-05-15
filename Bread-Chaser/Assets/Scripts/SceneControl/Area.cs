using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Area : MonoBehaviour
{
    public float        AreaSize { get; protected set; }
    private float       _totalLength;
    private void Awake()
    {
        AreaSize = gameObject.GetComponent<BoxCollider>().size.z * gameObject.transform.localScale.z;
        
    }

    private void Start()
    {
        _totalLength = Managers.Area.totalLength;
    }

    private void Update()
    {
        if(Managers.Scene.CurrentScene.Player.transform.position.z - (gameObject.transform.position.z + AreaSize) >= 15f)
        {
            Managers.Area.SpawnArea(Managers.Scene.GetSceneName(Define.Scene.City), ref Managers.Area.totalLength);
            Managers.Resource.Destroy(gameObject);
        }
    }
}
