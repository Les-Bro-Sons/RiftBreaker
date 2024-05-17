using UnityEngine;

public class RB_PointInTime
{
    public Vector3 Position;
    public Quaternion Rotation;

    public RB_PointInTime(Vector3 position, Quaternion rotation)
    {
        this.Position = position;
        this.Rotation = rotation;
    }
}