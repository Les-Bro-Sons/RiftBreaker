using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

public class RB_InputToolTip : MonoBehaviour {

    [Tooltip("Reference to action that is to be displayed from the UI.")]
    [SerializeField] InputActionReference _action;

    [SerializeField] string _bindingID;

    [SerializeField] TextMeshProUGUI _bindingText;

    [SerializeField] Image _bindingImage;

    [SerializeField] UpdateBindingUIEvent _updateBindingUIEvent;

    /// <summary>
    /// ID (in string form) of the binding that is to be rebound on the action.
    /// </summary>
    /// <seealso cref="InputBinding.id"/>
    public string bindingId {
        get => _bindingID;
        set {
            _bindingID = value;
            UpdateBindingDisplay();
        }
    }

    /// <summary>
    /// Text component that receives the display string of the binding. Can be <c>null</c> in which
    /// case the component entirely relies on <see cref="updateBindingUIEvent"/>.
    /// </summary>
    public TextMeshProUGUI bindingText {
        get => _bindingText;
        set {
            _bindingText = value;
            UpdateBindingDisplay();
        }
    }

    public Image bindingImage { 
        get => _bindingImage;
        set {
            _bindingImage = value;
            UpdateBindingDisplay();
        }
    }

    /// <summary>
    /// Event that is triggered every time the UI updates to reflect the current binding.
    /// This can be used to tie custom visualizations to bindings.
    /// </summary>
    public UpdateBindingUIEvent updateBindingUIEvent { 
        get { 
            if(_updateBindingUIEvent == null) { 
                _updateBindingUIEvent = new UpdateBindingUIEvent();
            }
            return _updateBindingUIEvent;
        }
    }

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

        // Set on label (if any).
        if (_bindingText != null){
            _bindingText.text = displayString;
        }

        // Give listeners a chance to configure UI in response.
        _updateBindingUIEvent?.Invoke(this, displayString, deviceLayoutName, controlPath);
    }

    public void Update() {
        UpdateBindingDisplay();
    }

#if UNITY_EDITOR
    protected void OnValidate() {
        UpdateBindingDisplay();
    }

#endif

    [Serializable] public class UpdateBindingUIEvent : UnityEvent<RB_InputToolTip, string, string, string> { }
}