using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RB_MenuControlManager : MonoBehaviour {
    [SerializeField] GameObject _keybinderKeyboard, _keybinderController;

    public enum BINDERS { keyboard, controller }
    public BINDERS CurrentBinder;

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
}
