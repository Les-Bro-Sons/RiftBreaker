using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RB_Health : MonoBehaviour {
    private float _hp;
    [SerializeField] float _hpMax;
    public float Armor;

    public UnityEvent EventDeath;

    public void TakeDamage(float amount) { 
        _hp -= amount;
    }

    public void Heal(float amount) { 
        _hp += amount;
    }
    
    public void Heal() {
        _hp = _hpMax;
    }
}
