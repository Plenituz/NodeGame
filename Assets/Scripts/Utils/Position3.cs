using System;
using UnityEngine;

[Serializable]
public class Position3
{
    public float x, y, z;

    public Position3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 ToVector()
    {
        return new Vector3(x, y, z);
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ", " + z + ")";
    }
}