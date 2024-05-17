using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RB_HUDRecast : MonoBehaviour {
    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] Image _fillImage;
    [SerializeField] Image _displayCast;

    float _remainTime;
    bool _isTimerStarted;
    float _multiplierFactor;

    public UnityEvent EventTimerEnd;

    private void Start() {
        _multiplierFactor = 1f / _remainTime;
        _timerText.text = "";
    }

    public void RecastTimerStart(float timer) {
        _multiplierFactor = 1f/timer;
        _remainTime = timer;
        _timerText.text = _remainTime.ToString();
        _isTimerStarted = true;
        _fillImage.fillAmount = _remainTime * _multiplierFactor;
    }

    void Update() { 
        if (!_isTimerStarted) { return; }

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

    void RecastTimerEnd() {
        _timerText.text = "";
        EventTimerEnd?.Invoke();
        _displayCast.color = Color.white;
    }

}
