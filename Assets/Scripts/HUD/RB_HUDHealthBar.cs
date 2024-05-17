using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RB_HUDHealthBar : MonoBehaviour {
    [SerializeField] Slider _hpBar;
    [SerializeField] TextMeshProUGUI _hpText;
    [SerializeField] RB_Health _rb_Health;

    [SerializeField] bool _isBoss;
    [SerializeField] TextMeshProUGUI _bossName;

    private void Start() {
        //Pour avoir le nom du boss au dessus de sa barre de vie
        if (_isBoss && _rb_Health.Name != null) { 
            _bossName.text = _rb_Health.Name;
        }
        //Le joueur ne possède pas de système d'affichage de son nom
        else if( _bossName != null) {
            _bossName.text = "";
        }

        //Quand les Event sont Invoke on Mets à jour la barre de vie
        _rb_Health.EventTakeDamage.AddListener(RefreshHealth);
        _rb_Health.EventHeal.AddListener(RefreshHealth);
    }

    //Mise à jour de la barre de vie
    void RefreshHealth(){
        _hpBar.value = _rb_Health.Hp / _rb_Health.HpMax;
        _hpText.text = _rb_Health.Hp.ToString() + " / " + _rb_Health.HpMax.ToString();
    }
}