using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    #region Inside Class Pool
    class Pool
    {
        public GameObject Original { get; private set; }
        public Transform Root { get; set; }

        Stack<Poolable> _poolStack = new Stack<Poolable>();

        public void Init(GameObject original, int count = 3)
        {
            Original = original;
            Root = new GameObject().transform;
            Root.name = $"{original.name}_Root";

            for (int i = 0; i < count; i++)
                Push(Create());
        }

        Poolable Create()
        {
            GameObject go = Object.Instantiate<GameObject>(Original);
            go.name = Original.name;

            return go.GetOrAddComponent<Poolable>();
            
                
        }

        public void Push(Poolable poolable)
        {
            if (poolable == null)
                return;

            poolable.transform.SetParent(Root);
            poolable.gameObject.SetActive(false);
            poolable.isUsing = false;

            _poolStack.Push(poolable);
        }

        public Poolable Pop(Transform parent)
        {
            Poolable poolable;

            if (_poolStack.Count > 0)
                poolable = _poolStack.Pop();
            else
                poolable = Create();

            poolable.gameObject.SetActive(true);

            if(parent != null)
                poolable.transform.parent = Managers.Scene.CurrentScene.transform;

            poolable.transform.SetParent(parent);
            poolable.isUsing = true;

            return poolable;
        }
    }
    #endregion

    #region variables
    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();
    Transform _root;
    #endregion

    #region Initialize
    public void Init()
    {
        if(_root == null)
        {
            _root = new GameObject { name = "@Pool_Root" }.transform;
            Object.DontDestroyOnLoad(_root);
        }
    }
    #endregion

    #region Pool Control
    // Create and add GameObjects into Pool ===================================
    public void CreatePool(GameObject original, int count = 3)
    {
        Pool pool = new Pool();
        pool.Init(original, count);
        pool.Root.parent = _root;

        _pool.Add(original.name, pool);
    }

    //Hiding Poolables ===========================================
    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;

        if(_pool.ContainsKey(name) == false)
        {
            GameObject.Destroy(poolable.gameObject);
            return;
        }

        _pool[name].Push(poolable);
    }

    //Activing Poolables ========================================
    public Poolable Pop(GameObject original, Transform parent = null, int count = 3)
    {
        if (_pool.ContainsKey(original.name) == false)
            CreatePool(original, count);

        return _pool[original.name].Pop(parent);
    }
    #endregion

    #region Check Pool & Clear
    public GameObject GetOriginal(string name)
    {
        if (_pool.ContainsKey(name) == false)
            return null;

        return _pool[name].Original;
    }

    public void Clear()
    {
        foreach(Transform child in _root)
            GameObject.Destroy(child.gameObject);

        _pool.Clear();
    }
    #endregion
}
