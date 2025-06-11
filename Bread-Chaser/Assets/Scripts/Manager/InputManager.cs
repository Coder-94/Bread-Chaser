using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Windows;

public class InputManager
{
    #region variables
    public Action<Define.TouchEvent> TouchAction = null;

    bool _pressed = false;
    float _pressedTime = 0;
    #endregion

    #region ForUpdate
    public void OnUpdate()
    {
        //드래그 추가
        if (TouchAction != null)
        {
            if (UnityEngine.Input.touchCount > 0)
            {
                if (!_pressed)
                {
                    TouchAction.Invoke(Define.TouchEvent.FingerPressed);
                    _pressedTime = Time.time;
                }
                TouchAction.Invoke(Define.TouchEvent.Holding);
                _pressed = true;
            }
            else
            {
                if (_pressed)
                {
                    if (Time.time < _pressedTime + 0.2f)
                    {
                        TouchAction.Invoke(Define.TouchEvent.Tap);
                    }
                    TouchAction.Invoke(Define.TouchEvent.FingerReleased);
                }

                _pressed = false;
                _pressedTime = 0;
            }
        }
    }
    #endregion

    #region Clear
    public void Clear()
    {
        TouchAction = null;
    }
    #endregion
}
