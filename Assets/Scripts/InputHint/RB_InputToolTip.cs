using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RB_InputToolTIp : MonoBehaviour {

    [Tooltip("Reference to action that is to be displayed from the UI.")]
    [SerializeField] InputActionReference _action;

    [SerializeField] string _bindingID;

    [SerializeField] TextMeshPro _bindingText;

    /// <summary>
    /// Trigger a refresh of the currently displayed binding.
    /// </summary>
    public void UpdateBindingDisplay() {
        string displayString = string.Empty;
        string deviceLayoutName = default;
        string controlPath = default;

        //Get display string from action.
        InputAction action = _action?.action;
        if (action != null) {
            int bindingIndex = action.bindings.IndexOf(u => u.id.ToString() == _bindingID);
            if (bindingIndex != -1) {
                displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath);
            }
        }

        if (_bindingText != null){
            _bindingText.text = displayString;
        }
    }


}
