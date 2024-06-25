using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RB_InputToolTipCombo : MonoBehaviour {
    [Tooltip("Reference to action that is to be displayed from the UI.")]
    [SerializeField] InputActionReference _action;

    [SerializeField] string _bindingID;
    [SerializeField] string _modifierID;

    [SerializeField] TextMeshProUGUI _bindingText;
    [SerializeField] TextMeshProUGUI _modifierText;

    [SerializeField] Image _bindingImage;
    [SerializeField] Image _modifierImage;

    [SerializeField] UpdateBindingUIEvent _updateBindingUIEvent;
    [SerializeField] UpdateModifierUIEvent _updateModifierUIEvent;

    [SerializeField] GameObject _textParent;
    [SerializeField] GameObject _imageParent;

    /// <summary>
    /// ID (in string form) of the binding that is to be rebound on the action.
    /// </summary>
    /// <seealso cref="InputBinding.id"/>
    public string bindingID
    {
        get => _bindingID;
        set
        {
            _bindingID = value;
            UpdateBindingDisplay();
        }
    }
    public string modifierID
    {
        get => _modifierID;
        set
        {
            _modifierID = value;
            UpdateBindingDisplay();
        }
    }

    /// <summary>
    /// Text component that receives the display string of the binding. Can be <c>null</c> in which
    /// case the component entirely relies on <see cref="updateBindingUIEvent"/>.
    /// </summary>
    public TextMeshProUGUI bindingText
    {
        get => _bindingText;
        set
        {
            _bindingText = value;
            UpdateBindingDisplay();
        }
    }
    public TextMeshProUGUI modifierText
    {
        get => _modifierText;
        set
        {
            _modifierText = value;
            UpdateBindingDisplay();
        }
    }

    public Image bindingImage
    {
        get => _bindingImage;
        set
        {
            _bindingImage = value;
            UpdateBindingDisplay();
        }
    } 
    public Image modifierImage
    {
        get => _modifierImage;
        set
        {
            _modifierImage = value;
            UpdateBindingDisplay();
        }
    }

    /// <summary>
    /// Event that is triggered every time the UI updates to reflect the current binding.
    /// This can be used to tie custom visualizations to bindings.
    /// </summary>
    public UpdateBindingUIEvent updateBindingUIEvent
    {
        get
        {
            if (_updateBindingUIEvent == null)
            {
                _updateBindingUIEvent = new UpdateBindingUIEvent();
            }
            return _updateBindingUIEvent;
        }
    }
    public UpdateModifierUIEvent updateModifierUIEvent
    {
        get
        {
            if (_updateModifierUIEvent == null)
            {
                _updateModifierUIEvent = new UpdateModifierUIEvent();
            }
            return _updateModifierUIEvent;
        }
    }

    public GameObject TextParent { 
        get => _textParent;
    }

    public GameObject ImageParent { 
        get => _imageParent;
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
        if (action != null)
        {
            int bindingIndex = action.bindings.IndexOf(u => u.id.ToString() == _bindingID);
            if (bindingIndex != -1)
            {
                displayString = action.GetBindingDisplayString(bindingIndex, out deviceLayoutName, out controlPath);
            }
        }

        // Set on label (if any).
        if (_bindingText != null)
        {
            _bindingText.text = displayString;
        }

        // Give listeners a chance to configure UI in response.
        _updateBindingUIEvent?.Invoke(this, displayString, deviceLayoutName, controlPath);
    }
    public void UpdateModifierDisplay() {
        string displayString = string.Empty;
        string deviceLayoutName = default;
        string controlPath = default;

        //Get display string from action.
        InputAction action = _action?.action;
        if (action != null)
        {
            int modifierIndex = action.bindings.IndexOf(u => u.id.ToString() == _modifierID);
            if (modifierIndex != -1)
            {
                displayString = action.GetBindingDisplayString(modifierIndex, out deviceLayoutName, out controlPath);
            }
        }

        // Set on label (if any).
        if (_modifierText != null)
        {
            _modifierText.text = displayString;
        }

        // Give listeners a chance to configure UI in response.
        _updateModifierUIEvent?.Invoke(this, displayString, deviceLayoutName, controlPath);
    }

    public void Update()
    {
        UpdateBindingDisplay();
        UpdateModifierDisplay();
    }

#if UNITY_EDITOR
    protected void OnValidate()
    {
        UpdateBindingDisplay();
        UpdateModifierDisplay();
    }

#endif

    [Serializable] public class UpdateBindingUIEvent : UnityEvent<RB_InputToolTipCombo, string, string, string> { }
    [Serializable] public class UpdateModifierUIEvent : UnityEvent<RB_InputToolTipCombo, string, string, string> { }
}
