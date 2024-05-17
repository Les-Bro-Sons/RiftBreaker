using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class RB_Health : MonoBehaviour {
    [SerializeField] float _hp; public float Hp { get { return _hp; } }
    [SerializeField] float _hpMax; public float HpMax { get { return _hpMax; } }
    public float Armor;

    //Nom de l'entit� qui poss�de le script
    string _name; public string Name { get { return _name; } }

    public UnityEvent EventDeath;
    public UnityEvent EventTakeDamage;
    public UnityEvent EventHeal;



    void Awake() {
        _name = gameObject.name;
        Heal();
    }

    //Fonction de prise de d�g�ts
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

    //Fonction de soin
    public void Heal(float amount) {
        if(_hp + amount > _hpMax) {
            _hp += amount;
        }
        else {
            Heal();
        }
        EventHeal.Invoke();
    }
    
    //Fonction de soin Maximum
    public void Heal() {
        _hp = _hpMax;
        EventHeal.Invoke();
    }

    //Permet d'emp�cher que les pv soit sup�rieur au pv max
    void Update() {
        if(_hp > _hpMax) { 
            _hp = _hpMax;
        }
    }

}
