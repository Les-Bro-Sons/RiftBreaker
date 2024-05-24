using UnityEngine;

public struct PointInTime
{
    public float Time;

    //TRANSFORM//
    public Vector3 Position;
    public Quaternion Rotation;
    /////////////

    public Sprite Sprite;

    //RB_Health//
    public float Health;
    public bool Dead;
    /////////////
}

public struct EventInTime
{
    public TYPETIMEEVENT TypeEvent;

    public float Time;

    public Vector3 Position;
    public Quaternion Rotation;
}

public enum TYPETIMEEVENT
{
    DestroyedPrefab,
}