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
                break;
            case RECASTTYPE.AttackCharged:
                break;
            case RECASTTYPE.AttackSpecial:
                break;
        }

    }

    //Initilisation du system de Recast Time
    public void RecastTimerStart(float timer) {
        _multiplierFactor = 1f/timer;
        _remainTime = timer;
        _timerText.text = _remainTime.ToString();
        _isTimerStarted = true;
        _fillImage.fillAmount = _remainTime * _multiplierFactor;
    }

    void Update() { 
        if (!_isTimerStarted) { return; }

        //Défilement du Recast Time
        if(_remainTime > 0f) {
            _displayCast.color = Color.gray;
            _remainTime -= Time.deltaTime;
            _timerText.text = _remainTime.ToString("0.0");
            _fillImage.fillAmount = _remainTime * _multiplierFactor;
        }
        else { 
            RecastTimerEnd();
        }
    }

    //Fonction qui se fait appeler quand la compétence à totalement rechargé. 
    void RecastTimerEnd() {
        _timerText.text = "";
        EventTimerEnd?.Invoke();
        _displayCast.color = Color.white;
    }

}
