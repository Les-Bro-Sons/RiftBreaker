using System.Collections.Generic;
using UnityEngine;

public class RB_EnemyCollisionSetLayer : MonoBehaviour
{
    CapsuleCollider _triggerCollider;
    CapsuleCollider _collider;

    private void Start()
    {
        SetCollisionLayer();

        if (RB_Tools.TryGetComponentInParent<RB_Health>(gameObject, out RB_Health health))
        {
            health.EventResurect.AddListener(SetCollisionLayer);
        }
    }

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
