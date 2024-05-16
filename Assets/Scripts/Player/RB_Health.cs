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
        if(_hp - amount > 0) {
            _hp -= amount;
        }
        else { 
            EventDeath.Invoke();
        }
    }

    public void Heal(float amount) {
        if(_hp + amount < _hpMax) {
            _hp += amount;
        }
        else { 
            _hp = _hpMax;
        }
    }
    
    public void Heal() {
        _hp = _hpMax;
    }
}
