using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RB_MenuControlManager : MonoBehaviour {
    [SerializeField] GameObject _keybinderKeyboard, _keybinderController;
    [SerializeField] GameObject _panel, _resetKeyboard, _resetController;
    public enum BINDERS { keyboard, controller }
    public BINDERS CurrentBinder;

    bool _isPanelOpen => _panel.activeSelf;

    public static RB_MenuControlManager Instance;

    private void Start(){
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    public void KeybindController() {
        _keybinderKeyboard.SetActive(false);
        _keybinderController.SetActive(true);
        CurrentBinder = BINDERS.controller;
    }

    public void KeybindKeyBoard() { 
        _keybinderKeyboard.SetActive(true);
        _keybinderController.SetActive(false);
        CurrentBinder = BINDERS.keyboard;
    }

    private void Update() {
        if (_isPanelOpen) { 
            _resetController.SetActive(false);
            _resetKeyboard.SetActive(false);
        }
        else {
            _resetController.SetActive(true);
            _resetKeyboard.SetActive(true);
        }
    }
}
