using System.Collections.Generic;
using UnityEngine;

public class RB_EnemyCollisionSetLayer : MonoBehaviour
{
    CapsuleCollider _triggerCollider;
    CapsuleCollider _collider;

    private void Start()
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
