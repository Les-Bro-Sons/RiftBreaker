using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_Spike : RB_Projectile
{
    public Collider SpikeCollider;
    [HideInInspector] public RB_Health Health;
    private void OnTriggerEnter(Collider other)
    {
        other = SpikeCollider;
        if (SpikeCollider.gameObject.CompareTag("Player"))
        {
            Health.TakeDamage(40);
        }
    }
}
