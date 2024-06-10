using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_PauseMenu : MonoBehaviour {

    [SerializeField] float _timeScaleSpeed;
    public bool IsPaused;
    [SerializeField] bool _isUnpausing;
    CanvasGroup _canvasGroup;

    void Start() {
        RB_MenuInputManager.Instance.EventPauseStarted.AddListener(Pause);
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update(){
        if (IsPaused) {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0f, Time.unscaledDeltaTime * _timeScaleSpeed);
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 1f, Time.unscaledDeltaTime * _timeScaleSpeed);

            if (Time.timeScale < 0.05f) {
                Time.timeScale = 0;
                
            }
        }
        else if (_isUnpausing) {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, Time.unscaledDeltaTime * _timeScaleSpeed);
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 0f, Time.unscaledDeltaTime * _timeScaleSpeed);
            RB_MenuManager.Instance.UnPauseAnim();

            if (Time.timeScale > 0.99f) {
                Time.timeScale = 1;
                _isUnpausing = false;
                _canvasGroup.alpha = 0f;
                RB_MenuManager.Instance.CloseOption();
                RB_ButtonSelectioner.Instance.SelectMainButton(0);
                RB_MenuManager.Instance.Animator.Play("UnPause");
            }
        }
        else {
            _canvasGroup.alpha = 0f;

        }
    }

    public void Pause() {
        if(!IsPaused){
            IsPaused = true;
            RB_MenuManager.Instance.PauseAnim();
            RB_ButtonSelectioner.Instance.SelectMainButton(0);
        }
        else {
            UnPause();
        }
    }

    public void UnPause() {
        RB_ButtonSelectioner.Instance.SelectMainButton(0);
        RB_MenuManager.Instance.BackMainMenu();
        RB_MenuManager.Instance.CancelQuit();
        IsPaused = false;
        _isUnpausing = true;
    }

}
