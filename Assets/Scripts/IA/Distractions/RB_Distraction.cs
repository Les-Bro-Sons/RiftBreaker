using System.Collections.Generic;
using UnityEngine;

public class RB_Distraction : MonoBehaviour
{
    [HideInInspector] public new Transform transform;

    public RB_DistractionData DistractionData;

    public DISTRACTIONTYPE DistractionType;
    public float Priority = 0;
    public float SoundRadiusWithoutWalls = 0;
    public float SoundRadiusWithWalls = 0;
    public bool RemoveSameDistraction = false;
    public bool IsVisible = false;

    /// <summary>
    /// Instantiate a new distraction
    /// </summary>
    /// <param name="distractionType">Type of the distraction</param>
    /// <param name="position">Position of the distraction</param>
    /// <param name="priority">Priority of the distraction</param>
    /// <param name="isVisible">Is the distraction visible or audio only</param>
    /// <param name="removeSameDistraction">Will remove distraction of the same type in the AI current distraction list</param>
    /// <param name="soundRadiusWithoutWalls">Range a guard hears the distraction if there are no walls between them and the distraction</param>
    /// <param name="soundRadiusWithWalls">Range a guard hears the distraction no matter what. By default, it is the normal range divided by 1.5</param>
    public static RB_Distraction NewDistraction(DISTRACTIONTYPE distractionType, Vector3 position, float priority, bool isVisible = false, bool removeSameDistraction = false, float soundRadiusWithoutWalls = 0, float? soundRadiusWithWalls = null)
    {
        RB_DistractionData distractionData = new();
        
        distractionData.IsVisible = isVisible;
        distractionData.RemoveSameDistraction = removeSameDistraction;
        distractionData.DistractionType = distractionType;
        distractionData.Priority = priority;
        distractionData.SoundRadiusWithoutWalls = soundRadiusWithoutWalls;
        if (soundRadiusWithWalls == null) soundRadiusWithWalls = soundRadiusWithoutWalls / 1.5f;
        distractionData.SoundRadiusWithWalls = soundRadiusWithWalls.Value;

        return NewDistraction(distractionData, position);
    }

    public static RB_Distraction NewDistraction(RB_DistractionData distractionData, Vector3 position)
    {
        GameObject distractionObject = new GameObject(distractionData.DistractionType.ToString() + " distraction");
        RB_Distraction distraction = distractionObject.AddComponent<RB_Distraction>();

        if (distractionData.IsVisible)
        {
            SphereCollider distractionCollider = distractionObject.AddComponent<SphereCollider>();
            Rigidbody distractionRb = distractionObject.AddComponent<Rigidbody>();
            distractionRb.useGravity = false;
            distractionRb.excludeLayers = ~12;
            distractionObject.layer = 12;
        }

        distraction.transform = distraction.GetComponent<Transform>();
        distraction.transform.position = position;
        distraction.DistractionData = distractionData;

        return distraction;
    }

    private void Start()
    {
        DistractionType = DistractionData.DistractionType;
        Priority = DistractionData.Priority;
        SoundRadiusWithoutWalls = DistractionData.SoundRadiusWithoutWalls;
        SoundRadiusWithWalls = DistractionData.SoundRadiusWithWalls;
        RemoveSameDistraction = DistractionData.RemoveSameDistraction;
        IsVisible = DistractionData.IsVisible;
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
                    ai.AddDistraction(this, RemoveSameDistraction);
                    hasDistracted = true;
                }
            }
        }
        if (!hasDistracted && !IsVisible) Destroy(gameObject);
    }

    public void OnDistractionCompleted()
    {

    }
}

[CreateAssetMenu(fileName = "DistractionData", menuName = "ScriptableObjects/RB_RB_DistractionData", order = 1)]
public class RB_DistractionData: ScriptableObject
{
    public DISTRACTIONTYPE DistractionType;
    public float Priority = 0;
    public float SoundRadiusWithoutWalls = 0;
    public float SoundRadiusWithWalls = 0;
    public bool RemoveSameDistraction = false;
    public bool IsVisible = false;
}
