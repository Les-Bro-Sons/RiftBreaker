using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public struct PointInTime
{
    public float Time;

    //TRANSFORM//
    public Vector3 Position;
    public Quaternion Rotation;
    /////////////

    //RIGIDBODY//
    public Vector3 Velocity;
    /////////////
   

    public Sprite Sprite;

    //RB_Health//
    public float Health;
    public bool Dead;
    /////////////

    //RB_LEVELMANAGER//
    public PHASES Phase;
    ///////////////////


    //RB_AI_BTTree//
    public Dictionary<string, bool> BoolDictionnary;
    /////////////

    public PointInTime InterpolateValues(PointInTime nextP, float currentTime)
    {
        PointInTime interpolatedP = this;

        float T1 = Time;
        float T2 = nextP.Time;


        Vector3 Pos1 = Position;
        Vector3 Pos2 = nextP.Position;
        interpolatedP.Position = Pos1 + (Pos2 - Pos1) * (currentTime - T1) / (T2 - T1);
        Quaternion Q1 = Rotation;
        Quaternion Q2 = nextP.Rotation;
        interpolatedP.Rotation = Quaternion.Slerp(Q1, Q2, (currentTime - T1) / (T2 - T1));

        return interpolatedP;
    }
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