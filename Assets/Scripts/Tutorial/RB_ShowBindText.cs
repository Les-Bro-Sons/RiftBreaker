using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

#if UNITY_EDITOR
[CustomEditor(typeof(RB_ShowBindText))]
public class RB_ShowBindTextEditor : UnityEditor.Editor
{

    #region Serialized Properties
    private SerializedProperty _actionProperty;
    private SerializedProperty _bindingIDProperty;
    private SerializedProperty _bindingTextProperty;
    private SerializedProperty _bindingImageProperty;
    private SerializedProperty _updateBindingUIEventProperty;


    private GUIContent _bindingLabel = new GUIContent("Binding");
    private GUIContent[] _bindingOptions;
    private int _selectedBindingOption;
    private string[] _bindingOptionsValues;
    #endregion

    protected void OnEnable()
    {
        _actionProperty = serializedObject.FindProperty("_action");
        _bindingIDProperty = serializedObject.FindProperty("_bindingID");
        _bindingTextProperty = serializedObject.FindProperty("_bindingText");
        _bindingImageProperty = serializedObject.FindProperty("_bindingImage");
        _updateBindingUIEventProperty = serializedObject.FindProperty("_updateBindingUIEvent");
        RefreshBindingOptions();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        // Display the label for the binding options
        EditorGUILayout.LabelField(_bindingLabel, EditorStyles.boldLabel);
        using (new EditorGUI.IndentLevelScope())
        {
            // Display the action property field
            EditorGUILayout.PropertyField(_actionProperty);

            // Display the popup for binding options
            var newSelectedBinding = EditorGUILayout.Popup(_bindingLabel, _selectedBindingOption, _bindingOptions);
            if (newSelectedBinding != _selectedBindingOption)
            {
                var bindingID = _bindingOptionsValues[newSelectedBinding];
                _bindingIDProperty.stringValue = bindingID;
                _selectedBindingOption = newSelectedBinding;
            }

            // Display the binding text property field
            EditorGUILayout.PropertyField(_bindingTextProperty);

            //Display the binding image property field
            EditorGUILayout.PropertyField(_bindingImageProperty);
        }

        // Apply any changes made in the inspector
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            RefreshBindingOptions();
        }
    }

    protected void RefreshBindingOptions()
    {
        var actionReference = (InputActionReference)_actionProperty.objectReferenceValue;
        var action = actionReference?.action;

        if (action == null)
        {
            // Clear binding options if no action is set
            _bindingOptions = new GUIContent[0];
            _bindingOptionsValues = new string[0];
            _selectedBindingOption = -1;
            return;
        }

        var bindings = action.bindings;
        var bindingCount = bindings.Count;

        _bindingOptions = new GUIContent[bindingCount];
        _bindingOptionsValues = new string[bindingCount];
        _selectedBindingOption = -1;

        var currentBindingId = _bindingIDProperty.stringValue;
        for (int i = 0; i < bindingCount; i++)
        {
            var binding = bindings[i];
            var bindingID = binding.id.ToString();
            var haveBindingsGroups = !string.IsNullOrEmpty(binding.groups);

            // Determine the display options for the binding
            var displayOptions = InputBinding.DisplayStringOptions.DontUseShortDisplayNames | InputBinding.DisplayStringOptions.IgnoreBindingOverrides;
            if (!haveBindingsGroups)
            {
                displayOptions |= InputBinding.DisplayStringOptions.DontOmitDevice;
            }

            // Create display string for the binding
            var displayString = action.GetBindingDisplayString(i, displayOptions);

            // Include part name if the binding is part of a composite
            if (binding.isPartOfComposite)
            {
                displayString = $"{ObjectNames.NicifyVariableName(binding.name)} : {displayString}";
            }

            // Prevent '/' from creating submenus in the popup
            displayString = displayString.Replace('/', '\\');

            // Mention control schemes if the binding is part of them
            if (haveBindingsGroups)
            {
                var asset = action.actionMap?.asset;
                if (asset != null)
                {
                    var controlSchemes = string.Join(", ", binding.groups.Split(InputBinding.Separator).Select(g => asset.controlSchemes.FirstOrDefault(s => s.bindingGroup == g).name));
                    displayString = $"{displayString} ({controlSchemes})";
                }
            }

            _bindingOptions[i] = new GUIContent(displayString);
            _bindingOptionsValues[i] = bindingID;

            if (currentBindingId == bindingID)
            {
                _selectedBindingOption = i;
            }
        }
    }
}
#endif

public class RB_ShowBindText : MonoBehaviour
{

    //Components
    private TextMeshProUGUI _currentText;

    [Tooltip("Reference to action that is to be displayed from the UI.")]
    [SerializeField] InputActionReference _action;

    [SerializeField] string _bindingID;

    [SerializeField] TextMeshProUGUI _bindingText;



    [SerializeField] Image _bindingImage;

    [SerializeField] UpdateBindingUIEvent _updateBindingUIEvent;

    public static string ReplaceTextBetweenBrackets(string inputText, string replacementText)
    {
        // Verify if string is empty or null
        if (string.IsNullOrEmpty(inputText) || string.IsNullOrEmpty(replacementText))
        {
            Debug.LogError("Le texte d'entrée ou le texte de remplacement est vide ou nul.");
            return inputText;
        }

        // Loop to replace all the occurences between brackets
        int startIndex = inputText.IndexOf('[');
        int endIndex = inputText.IndexOf(']');

        while (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
        {
            // Replace the between brackets text by the replacement text
            inputText = inputText.Substring(0, startIndex + 1) + replacementText + inputText.Substring(endIndex);

            // Find indexes for the next iteration
            startIndex = inputText.IndexOf('[', startIndex + replacementText.Length + 1);
            endIndex = startIndex != -1 ? inputText.IndexOf(']', startIndex) : -1;
        }

        return inputText;
    }

    

    /// <summary>
    /// ID (in string form) of the binding that is to be rebound on the action.
    /// </summary>
    /// <seealso cref="InputBinding.id"/>
    public string bindingId
    {
        get => _bindingID;
        set
        {
            _bindingID = value;
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

    public Image bindingImage
    {
        get => _bindingImage;
        set
        {
            _bindingImage = value;
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

    /// <summary>
    /// Trigger a refresh of the currently displayed binding.
    /// </summary>

    public void UpdateBindingDisplay()
    {
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
            _bindingText.text = $"[{displayString}]";
        }

        // Give listeners a chance to configure UI in response.
        _updateBindingUIEvent?.Invoke(this, displayString, deviceLayoutName, controlPath);
    }

    public void Update()
    {
        UpdateBindingDisplay();
    }

#if UNITY_EDITOR
    protected void OnValidate()
    {
        UpdateBindingDisplay();
    }

#endif

    [Serializable] public class UpdateBindingUIEvent : UnityEvent<RB_ShowBindText, string, string, string> { }
}
