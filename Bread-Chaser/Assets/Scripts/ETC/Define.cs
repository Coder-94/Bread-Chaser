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

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount
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

    public enum PlayerStatus
    {
        Running,
        Jumping,
        Attacking,
        BackStepping
    }

    public enum TouchEvent
    {
        FingerPressed,
        HoldedFingerReleased,
        FingerReleased,
        Tap,
        LeftTap,
        RightTap,
        Holding,
        LeftSwipe,
        RightSwipe,
        DownSwipe,
        UpSwipe
    }

    public struct SpawnedMobChecker
    {
        public GameObject   spawnedMob;
        public Vector3      spawnedPos;
    }

}