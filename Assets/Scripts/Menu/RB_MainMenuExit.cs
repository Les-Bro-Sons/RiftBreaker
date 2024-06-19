using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_MainMenuExit : MonoBehaviour {
    [SerializeField] GameObject _panel;

    bool _isPanelActive => _panel.activeSelf;
    void Start() {
        RB_MenuInputManager.Instance.EventPauseStarted.AddListener(CloseOption);
    }

    void CloseOption() {
        if (!_isPanelActive) {
            RB_MenuManager.Instance.BackMainMenu();
        }
    }

}
