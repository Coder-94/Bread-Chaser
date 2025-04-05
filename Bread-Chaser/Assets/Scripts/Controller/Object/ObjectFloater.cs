using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using UnityEngine.UIElements;

public class ObjectFloater : MonoBehaviour
{
    #region variables
    protected float       _bounciness = 0.1f;
    protected float       _frequency = 1f;
    protected Vector3     _posOffset = new Vector3();
    protected Vector3     _tempPos = new Vector3();
    #endregion

    protected virtual void Start()
    {
        _posOffset      = transform.position;
        
}

    protected virtual void Update()
    {
        

        _tempPos = _posOffset;
        _tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * _frequency) * _bounciness;

        transform.position = _tempPos;
    }
}
