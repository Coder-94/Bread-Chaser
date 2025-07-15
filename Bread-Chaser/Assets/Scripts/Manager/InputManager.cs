using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Windows;
using static BaseScene;

public class InputManager
{
    #region variables

    public Action<Define.TouchEvent, Vector2?>    TouchAction = null;

    float           _pressedTime = 0;
    bool            _pressed = false;
    bool            _holded = false;
    const float     DRAGDISTANCE = 100;
    Vector2         _touchStartPos;
    Vector2         _touchEndPos;
    Vector2         _swipeDelta;

    #endregion

    #region ForUpdate
    public void OnUpdate()
    {

        if (TouchAction != null)
        {
            if (UnityEngine.Input.touchCount > 0)
            {
                UnityEngine.Touch touch = UnityEngine.Input.GetTouch(0);

                //ui touch blocker ====================================================================
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;
                    

                //first touch input check ====================================================================
                if (!_pressed)
                {
                    _pressed = true;
                    _holded = false;
                    _pressedTime = Time.time;
                    _touchStartPos = touch.position;

                    TouchAction.Invoke(Define.TouchEvent.FingerPressed, null);
                }

                //swipe check ====================================================================
                if(touch.phase == TouchPhase.Moved)
                {
                    _touchEndPos = touch.position;
                    _swipeDelta = _touchEndPos - _touchStartPos;
                }

                //holding check ====================================================================
                if (Time.time > _pressedTime + 0.4f)
                {
                    if (!_holded)
                    {
                        TouchAction.Invoke(Define.TouchEvent.StartHolding, null);
                        _holded = true;
                    }
                        

                    TouchAction.Invoke(Define.TouchEvent.Holding, _swipeDelta);

                   
                }
            }
            else
            {
                //result check ====================================================================
                if (_pressed)
                {
                    //swipe result check ====================================================================
                    if (_swipeDelta.magnitude >= DRAGDISTANCE && !_holded)
                    {
                        if (Mathf.Abs(_swipeDelta.x) > Mathf.Abs(_swipeDelta.y))
                        {
                            if (_swipeDelta.x > 0)
                                TouchAction.Invoke(Define.TouchEvent.RightSwipe, null);
                            else
                                TouchAction.Invoke(Define.TouchEvent.LeftSwipe, null);
                        }
                        else
                        {
                            if (_swipeDelta.y > 0)
                                TouchAction.Invoke(Define.TouchEvent.UpSwipe, null);
                            else
                                TouchAction.Invoke(Define.TouchEvent.DownSwipe, null);
                        }
                    }

                    //holded result check ====================================================================
                    else if (_holded)
                    {
                        TouchAction.Invoke(Define.TouchEvent.HoldedFingerReleased, null);
                    }

                    //tapped result check ====================================================================
                    else
                    {
                        if (_touchStartPos.x - (Screen.width / 2) < 0)
                            TouchAction.Invoke(Define.TouchEvent.LeftTap, null);
                        else
                            TouchAction.Invoke(Define.TouchEvent.RightTap, null);

                        //TouchAction.Invoke(Define.TouchEvent.Tap);
                    }

                    TouchAction.Invoke(Define.TouchEvent.FingerReleased, null);
                }

                _pressed = false;
                _holded = false;
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
