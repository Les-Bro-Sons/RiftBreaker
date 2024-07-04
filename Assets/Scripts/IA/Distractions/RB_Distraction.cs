using System.Collections.Generic;
using UnityEngine;

public class RB_Distraction : MonoBehaviour
{
    [HideInInspector] public new Transform transform;

    public DISTRACTIONTYPE DistractionType;
    public float Priority = 0;
    public float SoundRadius = 0;

    /// <summary>
    /// Instantiate a new distraction
    /// </summary>
    public static RB_Distraction NewDistraction(DISTRACTIONTYPE distractionType, Vector3 position, float priority, float soundRadius)
    {
        GameObject distractionObject = new GameObject(distractionType.ToString() + " distraction");
        RB_Distraction distraction = distractionObject.AddComponent<RB_Distraction>();
        SphereCollider distractionCollider = distractionObject.AddComponent<SphereCollider>();
        Rigidbody distractionRb = distractionObject.AddComponent<Rigidbody>();
        distractionRb.useGravity = false;
        distractionRb.excludeLayers = ~12;
        distractionObject.layer = 12;

        distraction.transform = distraction.GetComponent<Transform>();
        distraction.transform.position = position;
        distraction.DistractionType = distractionType;
        distraction.Priority = priority;
        distraction.SoundRadius = soundRadius;

        return distraction;
    }

    private void Start()
    {
        OnDistractionSpawned();
    }

    private void OnDistractionSpawned()
    {
        foreach (Collider collider in Physics.OverlapSphere(transform.position, SoundRadius, ((1 << 9) | (1 << 6))))
        {
            if (RB_Tools.TryGetComponentInParent<RB_AI_BTTree>(collider.gameObject, out RB_AI_BTTree ai))
            {
                ai.AddDistraction(this);
            }
        }
    }
}
