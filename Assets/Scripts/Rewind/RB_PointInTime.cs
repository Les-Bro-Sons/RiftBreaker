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
    public float MaxHealth;
    public bool Dead;
    public TEAMS Team;
    /////////////

    //RB_LEVELMANAGER//
    public PHASES Phase;
    ///////////////////


    //RB_AI_BTTree//
    public Dictionary<BTBOOLVALUES, bool> BoolDictionnary;
    public float SpotValue;
    public int CurrentWaypointIndex;
    /////////////

    public List<EventInTime> TimeEvents;

    public PointInTime InterpolateValues(PointInTime nextP, float currentTime)
    {
        PointInTime interpolatedP = this;

        float T1 = Time;
        float T2 = nextP.Time;

        float spot1 = SpotValue;
        float spot2 = nextP.SpotValue;
        interpolatedP.SpotValue = spot1 + (spot2 - spot1) * (currentTime - T1) / (T2 - T1);

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

    //TookWeapon
    public RB_Items ItemTook;
}

public enum TYPETIMEEVENT
{
    TookWeapon,
    CloseDoor,
    OpenDoor,
}