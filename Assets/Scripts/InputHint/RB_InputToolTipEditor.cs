#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[CustomEditor(typeof(RB_InputToolTip))]
public class RB_InputToolTipEditor : UnityEditor.Editor
{

    #region Serialized Properties
    SerializedProperty _actionProperty;
    SerializedProperty _bindingIDProperty;
    SerializedProperty _bindingTextProperty;
    SerializedProperty _bindingImageProperty;
    SerializedProperty _updateBindingUIEventProperty;


    GUIContent _bindingLabel = new GUIContent("Binding");
    GUIContent[] _bindingOptions;
    int _selectedBindingOption;
    string[] _bindingOptionsValues;
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
