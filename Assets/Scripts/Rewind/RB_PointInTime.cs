using UnityEngine;

public class RB_PointInTime
{
    public float Time;
    public Vector3 Position;
    public Quaternion Rotation;

    public RB_PointInTime(Vector3 position, Quaternion rotation)
    {
        this.Position = position;
        this.Rotation = rotation;
    }

    public RB_PointInTime(float time, Vector3 position, Quaternion rotation)
    {
        this.Time = time;
        this.Position = position;
        this.Rotation = rotation;
    }
}