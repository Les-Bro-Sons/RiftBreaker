using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RB_Health : MonoBehaviour {
    [SerializeField] float _hp;
    [SerializeField] float _hpMax;
    public float Armor;

    public UnityEvent EventDeath;

    [SerializeField] Slider _hpBar;
    [SerializeField] TextMeshProUGUI _hpText;

    void Awake() { 
        Heal();
    }

    public void TakeDamage(float amount) {
        if(_hp - amount > 0) {
            _hp -= amount;
        }
        else { 
            EventDeath.Invoke();
        }
    }

    public void Heal(float amount) {
        if(_hp + amount > _hpMax) {
            _hp += amount;
        }
        else {
            Heal();
        }
    }
    
    public void Heal() {
        _hp = _hpMax;
    }

    void Update() {
        if (_hp > _hpMax) {
            Heal();
        }
        _hpBar.value = _hp / _hpMax;
        _hpText.text = _hp.ToString()+" / "+_hpMax.ToString();
    }
}
