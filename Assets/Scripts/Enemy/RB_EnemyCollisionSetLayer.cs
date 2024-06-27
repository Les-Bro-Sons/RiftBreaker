using UnityEngine;

public class RB_EnemyCollisionSetLayer : MonoBehaviour
{
    // Colliders used for setting collision layers
    private CapsuleCollider _triggerCollider;
    private CapsuleCollider _collider;

    /// <summary>
    /// Initializes the collision layers and sets up event listeners on Start.
    /// </summary>
    private void Start()
    {
        SetCollisionLayer();

        if (RB_Tools.TryGetComponentInParent<RB_Health>(gameObject, out RB_Health health))
        {
            health.EventResurect.AddListener(SetCollisionLayer);
        }
    }

    /// <summary>
    /// Configures the collision layers for the enemy's colliders.
    /// </summary>
    private void SetCollisionLayer()
    {
        int currentLayer = gameObject.layer;

        CapsuleCollider[] colliders = GetComponents<CapsuleCollider>();
        _triggerCollider = colliders[0];
        _collider = colliders[1];

        _triggerCollider.isTrigger = true;
        _collider.isTrigger = false;

        _collider.excludeLayers = (1 << currentLayer);
        _triggerCollider.excludeLayers = ~(1 << currentLayer);
    }
}
