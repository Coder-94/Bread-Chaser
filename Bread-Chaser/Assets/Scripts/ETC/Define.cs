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