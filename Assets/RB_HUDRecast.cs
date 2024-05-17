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
    bool startTimer;
    float multiplierFactor;

    public UnityEvent _timerEnd;

    private void Start() {
        multiplierFactor = 1f / _remainTime;
        _timerText.text = "";
    }

    public void RecastTimerStart(float timer) {
        multiplierFactor = 1f/timer;
        _remainTime = timer;
        _timerText.text = _remainTime.ToString();
        startTimer = true;
        _fillImage.fillAmount = _remainTime * multiplierFactor;
    }

    void Update() { 
        if (!startTimer) { return; }

        if(_remainTime > 0f) {
            _displayCast.color = Color.gray;
            _remainTime -= Time.deltaTime;
            _timerText.text = _remainTime.ToString("0.0");
            _fillImage.fillAmount = _remainTime * multiplierFactor;
        }
        else { 
            RecastTimerEnd();
        }
    }

    void RecastTimerEnd() {
        _timerText.text = "";
        _timerEnd?.Invoke();
        _displayCast.color = Color.white;
    }

}
