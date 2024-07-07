using System.Collections.Generic;
using UnityEngine;

public class RB_Distraction : MonoBehaviour
{
    [HideInInspector] public new Transform transform;

    public DISTRACTIONTYPE DistractionType;
    public float Priority = 0;
    public float SoundRadiusWithoutWalls = 0;
    public float SoundRadiusWithWalls = 0;

    private bool _isVisible = false;

    /// <summary>
    /// Instantiate a new distraction
    /// </summary>
    /// <param name="distractionType">Type of the distraction</param>
    /// <param name="position">Position of the distraction</param>
    /// <param name="priority">Priority of the distraction</param>
    /// <param name="soundRadiusWithoutWalls">Range a guard hears the distraction if there are no walls between them and the distraction</param>
    /// <param name="soundRadiusWithWalls">Range a guard hears the distraction no matter what. By default, it is the normal range divided by two</param>
    public static RB_Distraction NewDistraction(DISTRACTIONTYPE distractionType, Vector3 position, float priority, bool isVisible = false, float soundRadiusWithoutWalls = 10, float? soundRadiusWithWalls = null)
    {
        GameObject distractionObject = new GameObject(distractionType.ToString() + " distraction");
        RB_Distraction distraction = distractionObject.AddComponent<RB_Distraction>();
        
        distraction._isVisible = isVisible;
        if (isVisible)
        {
            SphereCollider distractionCollider = distractionObject.AddComponent<SphereCollider>();
            Rigidbody distractionRb = distractionObject.AddComponent<Rigidbody>();
            distractionRb.useGravity = false;
            distractionRb.excludeLayers = ~12;
            distractionObject.layer = 12;
        }

        distraction.transform = distraction.GetComponent<Transform>();
        distraction.transform.position = position;
        distraction.DistractionType = distractionType;
        distraction.Priority = priority;
        distraction.SoundRadiusWithoutWalls = soundRadiusWithoutWalls;
        if (soundRadiusWithWalls == null) soundRadiusWithWalls = soundRadiusWithoutWalls / 2f;
        distraction.SoundRadiusWithWalls = soundRadiusWithWalls.Value;

        return distraction;
    }

    private void Start()
    {
        OnDistractionSpawned();
    }

    private void OnDistractionSpawned()
    {
        bool hasDistracted = false;
        foreach (Collider collider in Physics.OverlapSphere(transform.position, SoundRadiusWithoutWalls, ((1 << 9) | (1 << 6))))
        {
            if (RB_Tools.TryGetComponentInParent<RB_AI_BTTree>(collider.gameObject, out RB_AI_BTTree ai))
            {
                Vector3 aiPos = ai.transform.position;
                Vector3 aiDir = aiPos - transform.position;
                float aiDistance = Vector3.Distance(aiPos, transform.position);
                if (aiDistance <= SoundRadiusWithWalls || (aiDistance <= SoundRadiusWithoutWalls && !Physics.Raycast(transform.position, aiDir.normalized, aiDir.magnitude, (1 <<3))))
                {
                    ai.AddDistraction(this);
                    hasDistracted = true;
                }
            }
        }
        if (!hasDistracted && !_isVisible) Destroy(gameObject);
    }
}
