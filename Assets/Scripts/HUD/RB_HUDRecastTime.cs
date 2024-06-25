using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RB_HUDRecastTime : MonoBehaviour {
    public enum RECASTTYPE
    {
        Dash,
        AttackBase,
        AttackCharged,
        AttackSpecial
    }

    public RECASTTYPE Type;

    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] Image _fillImage;
    [SerializeField] Image _displayCast;

    float _remainTime;
    bool _isTimerStarted;
    float _multiplierFactor;

    public UnityEvent EventTimerEnd;

    //Par Défaut le système de RecastTime (ou rechargement des compétences) n'est pas activé
    private void Start() {
        _multiplierFactor = 1f / _remainTime;
        _fillImage.fillAmount = 0;
        _timerText.text = "";
        switch (Type) {
            case RECASTTYPE.Dash:
                RB_PlayerMovement.Instance.EventDash.AddListener(delegate { RecastTimerStart(RB_PlayerMovement.Instance.DashCooldown); });
                break;
            case RECASTTYPE.AttackBase:
                RB_PlayerAction.Instance.EventBasicAttack.AddListener(delegate { RecastTimerStart(RB_PlayerAction.Instance.CurrentItem.AttackCooldown()); });
                break;
            case RECASTTYPE.AttackCharged:
                RB_PlayerAction.Instance.EventStartChargingAttack.AddListener(delegate { RecastTimerStart(RB_PlayerAction.Instance.CurrentItem.ChargeTime - RB_PlayerAction.Instance.StartChargingDelay); });
                RB_PlayerAction.Instance.EventStopChargingAttack.AddListener(RecastTimerEnd);
                break;
            case RECASTTYPE.AttackSpecial:
                _multiplierFactor = 1f / 100f;
                break;
        }

    }

    //Initilisation du system de Recast Time
    public void RecastTimerStart(float timer) {
        _multiplierFactor = 1f/timer;
        _remainTime = timer;
        _timerText.text = _remainTime.ToString();
        _isTimerStarted = true;
        switch (Type)
        {
            default:
                _fillImage.fillAmount = _remainTime * _multiplierFactor;
                break;
            case RECASTTYPE.AttackCharged:
                _fillImage.fillAmount = 1;
                break;
        }
        
    }

    void Update() { 
        switch (Type)
        {
            case RECASTTYPE.AttackSpecial:
                float charge = RB_PlayerAction.Instance.SpecialAttackCharge;
                if (charge < 100)
                {
                    //_displayCast.color = Color.gray;
                    _timerText.text = charge.ToString("0");
                    _fillImage.fillAmount = Mathf.Abs((charge * _multiplierFactor) - 1);
                }
                else
                {
                    //_displayCast.color = Color.white;
                    _timerText.text = "";
                    _fillImage.fillAmount = 0;
                }

                break;
        }

        if (!_isTimerStarted) { return; }

        //Défilement du Recast Time
        if(_remainTime > 0f) {
            //_displayCast.color = Color.gray;
            _remainTime -= Time.deltaTime;
            _timerText.text = _remainTime.ToString("0.0");
            switch (Type)
            {
                case RECASTTYPE.AttackCharged:
                    _fillImage.fillAmount = 1 - (_remainTime * _multiplierFactor);
                    break;
                default:
                    _fillImage.fillAmount = _remainTime * _multiplierFactor;
                    break;
            }
            
        }
        else { 
            RecastTimerEnd();
        }
    }

    //Fonction qui se fait appeler quand la compétence à totalement rechargé. 
    void RecastTimerEnd() {
        _timerText.text = "";
        _remainTime = 0;
        _fillImage.fillAmount = 0;
        EventTimerEnd?.Invoke();
        //_displayCast.color = Color.white;
    }

}
