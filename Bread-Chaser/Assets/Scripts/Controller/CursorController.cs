using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    Texture2D _icon;

    void Start()
    {
        _icon = Managers.Resource.Load<Texture2D>("Textures/Cursor");
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
            return;

        Cursor.SetCursor(_icon, new Vector2(_icon.width / 5, 0), CursorMode.Auto);

    }
}
