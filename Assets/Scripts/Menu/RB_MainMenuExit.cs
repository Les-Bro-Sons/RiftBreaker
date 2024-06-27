using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_MainMenuExit : MonoBehaviour
{
    [SerializeField] GameObject _panel;  // Reference to the panel that determines if the menu is active

    // Property to check if the panel is active
    bool _isPanelActive => _panel.activeSelf;

    void Start()
    {
        // Listen to the pause started event in the menu input manager
        RB_MenuInputManager.Instance.EventPauseStarted.AddListener(CloseOption);
    }

    // Method called when the pause event is triggered
    void CloseOption()
    {
        if (!_isPanelActive)
        {  // Check if the panel is not active
            RB_MenuManager.Instance.BackMainMenu();  // Call the menu manager to return to the main menu
        }
    }
}
