using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum UIEvent
    {
        Click,
        Drag
    }

    public enum Layer
    {
        Ground = 3,
        Wall = 6,
        Obstacle = 7,
        Player = 8,
        Enemy = 9
    }

    public enum Scene
    {
        Unknown,
        Main,
        City,
        Forest,
        IceLand,
        Universe,
        Score
    }

    public enum TouchEvent
    {
        FingerPressed,
        FingerReleased,
        Tap,
        Holding,
        LeftSwipe,
        RightSwipe,
        DownSwipe,
        UpSwipe
    }

}