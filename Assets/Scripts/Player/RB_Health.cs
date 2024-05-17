using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class RB_Health : MonoBehaviour {
    [SerializeField] float _hp; public float Hp { get { return _hp; } }
    [SerializeField] float _hpMax; public float HpMax { get { return _hpMax; } }
    public float Armor;

    [SerializeField] string _name; public string Name { get { return _name; } }

    public UnityEvent EventDeath;
    public UnityEvent EventTakeDamage;
    public UnityEvent EventHeal;



    void Awake() { 
        Heal();
    }

    public void TakeDamage(float amount) {
        if(_hp - amount > 0) {
            _hp -= amount;
        }
        else { 
            _hp = 0;
            EventDeath.Invoke();
        }
        EventTakeDamage.Invoke();
    }

    public void Heal(float amount) {
        if(_hp + amount > _hpMax) {
            _hp += amount;
        }
        else {
            Heal();
        }
        EventHeal.Invoke();
    }
    
    public void Heal() {
        _hp = _hpMax;
        EventHeal.Invoke();
    }

    void Update() {
        if(_hp > _hpMax) { 
            _hp = _hpMax;
        }
    }

}
