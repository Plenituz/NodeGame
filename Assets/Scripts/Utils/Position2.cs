using System;
using UnityEngine;

[Serializable]
public class Position2
{
    public float x, y;

    public Position2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public Position2(Vector2 vec)
    {
        x = vec.x;
        y = vec.y;
    }

    public Vector2 ToVector()
    {
        return new Vector2(x, y);
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }
}

