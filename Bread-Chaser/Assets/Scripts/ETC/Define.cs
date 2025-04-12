using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum MouseEvent
    {
        Press,
        PointerDown,
        PointerUp,
        Click
    }

    public enum BtnMoveEvt
    {
        Left,
        Right,
        ShortJump, Jump, LongJump
    }

    public enum Layer
    {
        Ground = 3,
        Wall = 6,
        Obstacle = 7,
        SlideObstacle = 8,
        CanParryAtk = 9,
        CantParryAtk = 10
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
}