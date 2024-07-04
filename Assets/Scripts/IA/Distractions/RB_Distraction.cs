using System.Collections.Generic;
using UnityEngine;

public class RB_Distraction
{
    public Vector3 Position;
    public DISTRACTIONTYPE DistractionType;
    public float Priority = 0;
    public float SoundRadius = 0;

    public RB_Distraction(DISTRACTIONTYPE distractionType, Vector3 position, float priority, float soundRadius)
    {
        Position = position;
        DistractionType = distractionType;
        Priority = priority;
        SoundRadius = soundRadius;
        OnDistractionSpawned();
    }

    public void OnDistractionSpawned()
    {
        foreach (Collider collider in Physics.OverlapSphere(Position, SoundRadius))
        {
            if (RB_Tools.TryGetComponentInParent<RB_AI_BTTree>(collider.gameObject, out RB_AI_BTTree ai) && !ai.Distractions.Contains(this))
            {
                ai.Distractions.Add(this);
            }
        }
    }
}
