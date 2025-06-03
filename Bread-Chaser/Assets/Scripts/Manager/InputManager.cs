using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class InputManager
{
    #region variables
    public Action KeyAction = null;
    #endregion

    #region ForUpdate
    public void OnUpdate()
    {
        if (Input.touchCount > 0)
            KeyAction.Invoke();
    }
    #endregion

    #region Clear
    public void Clear()
    {
        KeyAction = null;
    }
    #endregion
}
