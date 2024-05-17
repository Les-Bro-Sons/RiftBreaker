using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RB_HUDExitTimer : MonoBehaviour{

    float _remainTime;
    [SerializeField] TextMeshProUGUI _timerText;

    bool _isTimerStarted;

    public UnityEvent EventExitTimerEnd;
    private void Start() {
        _timerText.text = "";
    }

    //Fonction qui permet de lancer le timer de temps impartit pour s'enfuir
    public void ExitTimerStart(float timer){
        _remainTime = timer;
        _timerText.text = _remainTime.ToString();
        _isTimerStarted = true;
    }

    private void Update() {
        if (!_isTimerStarted) { return; }

        if(_remainTime > 0) { 
            _remainTime -= Time.deltaTime;
            //Format du Timer en Minutes : Secondes
            float mins = Mathf.FloorToInt(_remainTime / 60);
            float secs = Mathf.FloorToInt(_remainTime % 60);
            _timerText.text = string.Format("{0:00} : {1:00}", mins, secs);
        }
        else {
            ExitTimerEnd();
        }
    }

    //Fonction qui Invoke un Event quand le timer est fini.
    void ExitTimerEnd(){
        _timerText.text = "";
        EventExitTimerEnd?.Invoke();
    }
}
