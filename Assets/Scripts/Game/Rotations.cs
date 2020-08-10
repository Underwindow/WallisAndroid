using System.Collections.Generic;
using UnityEngine;

public enum RotatingDirection
{
    None,
    Left,
    Right,
    Up,
    Down,
    LeftAround,
    RightAround,
    Random
}

public class Rotations
{
    private readonly static Random random = new Random();

    public static Dictionary<RotatingDirection, Vector3> eulers = new Dictionary<RotatingDirection, Vector3>
    {
        { RotatingDirection.None, Vector3.zero },
        { RotatingDirection.Left, Vector3.up * 90 },
        { RotatingDirection.Right, Vector3.down * 90 },
        { RotatingDirection.Up, Vector3.right * 90 },
        { RotatingDirection.Down, Vector3.left * 90 },
        { RotatingDirection.LeftAround, Vector3.forward * 90 },
        { RotatingDirection.RightAround, Vector3.back * 90 },
        { RotatingDirection.Random, Vector3.one }
    };
}
