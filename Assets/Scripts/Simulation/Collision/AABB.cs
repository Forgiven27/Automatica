using UnityEngine;

public struct AABB
{
    public Vector3 Min;
    public Vector3 Max;

    public AABB(Vector3 min, Vector3 max)
    {
        Min = min; Max = max;
    }
    public bool Intersects(AABB other)
    {
        if (Max.x < other.Min.x || Min.x > other.Max.x)
            return false;

        if (Max.y < other.Min.y || Min.y > other.Max.y)
            return false;

        if (Max.z < other.Min.z || Min.z > other.Max.z)
            return false;

        return true;
    }
    public bool Contains(Vector3 point)
    {
        return
            point.x >= Min.x && point.x <= Max.x &&
            point.y >= Min.y && point.y <= Max.y &&
            point.z >= Min.z && point.z <= Max.z;
    }
    public void Normalize()
    {
        Min = Vector3.Min(Min, Max);
        Max = Vector3.Max(Min, Max);
    }
}
