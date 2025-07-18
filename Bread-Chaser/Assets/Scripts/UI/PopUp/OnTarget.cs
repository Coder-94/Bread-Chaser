using UnityEngine;

public class OnTarget: UIPopUp
{
    enum GameObjects
    {
        OnTarget,
        OnTargetCursor
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject SetCursor()
    {
        GameObject cursor = GetObject((int)GameObjects.OnTargetCursor).gameObject;

        if (cursor != null)
            return cursor;

        return null;
    }
}
