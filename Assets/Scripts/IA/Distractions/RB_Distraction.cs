using UnityEngine;

public class RB_Distraction
{
    public Vector3 Position;
    public DISTRACTIONTYPE DistractionType;

    public RB_Distraction(DISTRACTIONTYPE distractionType, Vector3 position)
    {
        Position = position;
        DistractionType = distractionType;
    }

    public void OnDistractionSpawned()
    {

    }
}
