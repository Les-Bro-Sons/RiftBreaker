using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_MenuControlManager : MonoBehaviour {
    [SerializeField] GameObject _keybinderKeyboard, _keybinderController;

    public void KeybindController() {
        _keybinderKeyboard.SetActive(false);
        _keybinderController.SetActive(true);
    }

    public void KeybindKeyBoard() { 
        _keybinderKeyboard.SetActive(true);
        _keybinderController.SetActive(false);
    }
}
