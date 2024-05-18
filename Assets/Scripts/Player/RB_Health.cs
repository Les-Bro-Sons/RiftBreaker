using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class RB_Health : MonoBehaviour {
    [SerializeField] float _hp; public float Hp { get { return _hp; } }
    [SerializeField] float _hpMax; public float HpMax { get { return _hpMax; } }
    public float Armor;

    //Nom de l'entité qui possède le script
    string _name; public string Name { get { return _name; } }

    public UnityEvent EventDeath;
    public UnityEvent EventTakeDamage;
    public UnityEvent EventHeal;


    // ~~~~~~~~~~ UX ~~~~~~~~~~
    public float LerpTimer;



    void Awake() {
        _name = gameObject.name;
        Heal();
    }

    //Fonction de prise de dégâts
    public void TakeDamage(float amount) {
        if(_hp - amount > 0) {
            _hp -= amount;
        }
        else { 
            _hp = 0;
            EventDeath.Invoke();
        }
        EventTakeDamage.Invoke();
        LerpTimer = 0.0f;
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
        LerpTimer = 0.0f;
    }

    //Fonction de soin Maximum
    public void Heal() {
        _hp = _hpMax;
        EventHeal.Invoke();
        LerpTimer = 0.0f;
    }

    //Permet d'empêcher que les pv soit supérieur au pv max
    void Update() {

        if (Input.GetKeyUp(KeyCode.H))
            TakeDamage(15);

        if (Input.GetKeyUp(KeyCode.J))
            Heal(15);

        if(_hp >= _hpMax)
            _hp = _hpMax;

    }
}