using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_TestDamage : MonoBehaviour
{

    [SerializeField] bool damage;
    [SerializeField] RB_Health health;
    [SerializeField] int damageamount;

    // Update is called once per frame
    void Update()
    {
        if (damage)
        {
            health.TakeDamage(damageamount);
            damage = false;
        }
    }
}
