using UnityEngine;

public class RB_MenuControlManager : MonoBehaviour
{
    [SerializeField] GameObject _keybinderKeyboard, _keybinderController;  // References to UI panels for keyboard and controller keybinding
    [SerializeField] GameObject _panel, _resetKeyboard, _resetController;  // References to UI elements for panel, and reset buttons
    public enum BINDERS { keyboard, controller }  // Enum defining keyboard and controller binders
    public BINDERS CurrentBinder;  // Current selected binder

    // Property to check if the panel is open
    bool _isPanelOpen => _panel.activeSelf;

    public static RB_MenuControlManager Instance;  // Singleton instance reference

    private void Start()
    {
        if (Instance == null) { Instance = this; }  // Set singleton instance
        else { Destroy(gameObject); }  // Destroy duplicate instances
    }

    // Switch to controller keybinding panel
    public void KeybindController()
    {
        _keybinderKeyboard.SetActive(false);  // Disable keyboard keybinding panel
        _keybinderController.SetActive(true);  // Enable controller keybinding panel
        CurrentBinder = BINDERS.controller;  // Set current binder to controller
    }

    // Switch to keyboard keybinding panel
    public void KeybindKeyBoard()
    {
        _keybinderKeyboard.SetActive(true);  // Enable keyboard keybinding panel
        _keybinderController.SetActive(false);  // Disable controller keybinding panel
        CurrentBinder = BINDERS.keyboard;  // Set current binder to keyboard
    }

    private void Update()
    {
        if (_isPanelOpen)
        {
            _resetController.SetActive(false);  // If panel is open, disable reset controller button
            _resetKeyboard.SetActive(false);   // If panel is open, disable reset keyboard button
        }
        else
        {
            _resetController.SetActive(true);   // If panel is closed, enable reset controller button
            _resetKeyboard.SetActive(true);    // If panel is closed, enable reset keyboard button
        }
    }
}
