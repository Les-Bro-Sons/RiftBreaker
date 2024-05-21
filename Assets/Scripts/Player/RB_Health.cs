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
    
    [HideInInspector] public UnityEvent EventDeath;
    [HideInInspector] public UnityEvent EventTakeDamage;
    [HideInInspector] public UnityEvent EventHeal;

    
    // ~~~~~~~~~~ UX ~~~~~~~~~~
    public float LerpTimer; //A METTRE DANS LE HUD



    void Awake() {
        _name = gameObject.name;
        _hp = _hpMax;
    }

    //Fonction de prise de dégâts
    public void TakeDamage(float amount) {
        _hp = Mathf.Clamp(_hp - amount, 0, _hpMax);
        LerpTimer = 0.0f;
        EventTakeDamage.Invoke();
        if (_hp <= 0)
            EventDeath.Invoke();
    }

    //Fonction de soin
    public void Heal(float amount) {
        _hp = Mathf.Clamp(_hp + amount, 0, _hpMax);
        LerpTimer = 0.0f;
        EventHeal.Invoke();
    }

    //Fonction de soin Maximum
    public void Heal() {
        _hp = _hpMax;
        LerpTimer = 0.0f;
        EventHeal.Invoke();
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