using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RB_HUDHealthBar : MonoBehaviour
{
    [SerializeField] Slider _hpBar;
    [SerializeField] TextMeshProUGUI _hpText;
    [SerializeField] RB_Health _rb_Health;

    [SerializeField] bool _isBoss;
    [SerializeField] TextMeshProUGUI _bossName;

    private void Start() {
        if (_isBoss && _rb_Health.Name != null) { 
            _bossName.text = _rb_Health.Name;
        }
        else if( _bossName != null) {
            _bossName.text = "";
        }
        _rb_Health.EventTakeDamage.AddListener(RefreshHealth);
        _rb_Health.EventHeal.AddListener(RefreshHealth);
    }

    void RefreshHealth(){
        _hpBar.value = _rb_Health.Hp / _rb_Health.HpMax;
        _hpText.text = _rb_Health.Hp.ToString() + " / " + _rb_Health.HpMax.ToString();
    }
}