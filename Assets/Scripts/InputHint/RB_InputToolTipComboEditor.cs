#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[CustomEditor(typeof(RB_InputToolTipCombo))]
public class RB_InputToolTipComboEditor : UnityEditor.Editor
{
    #region Serialized Properties
    SerializedProperty _actionProperty;
    SerializedProperty _bindingIDProperty;
    SerializedProperty _modifierIDProperty;
    SerializedProperty _bindingTextProperty;
    SerializedProperty _modifierTextProperty;
    SerializedProperty _bindingImageProperty;
    SerializedProperty _modifierImageProperty;
    SerializedProperty _updateBindingUIEventProperty;
    SerializedProperty _updateModifierUIEventProperty;

    SerializedProperty _textParentProperty;
    SerializedProperty _imageParentProperty;

    GUIContent _bindingLabel = new GUIContent("Binding");
    GUIContent _modifierLabel = new GUIContent("Modifier");

    GUIContent[] _bindingOptions;
    int _selectedBindingOption;
    string[] _bindingOptionsValues;

    GUIContent[] _modifierOptions;
    int _selectedModifierOption;
    string[] _modifierOptionsValues;
    #endregion

    protected void OnEnable()
    {
        _actionProperty = serializedObject.FindProperty("_action");
        _bindingIDProperty = serializedObject.FindProperty("_bindingID");
        _modifierIDProperty = serializedObject.FindProperty("_modifierID");
        _bindingTextProperty = serializedObject.FindProperty("_bindingText");
        _modifierTextProperty = serializedObject.FindProperty("_modifierText");
        _bindingImageProperty = serializedObject.FindProperty("_bindingImage");
        _modifierImageProperty = serializedObject.FindProperty("_modifierImage");
        _updateBindingUIEventProperty = serializedObject.FindProperty("_updateBindingUIEvent");
        _updateModifierUIEventProperty = serializedObject.FindProperty("_updateModifierUIEvent");

        _textParentProperty = serializedObject.FindProperty("_textParent");
        _imageParentProperty = serializedObject.FindProperty("_imageParent");

        RefreshBindingOptions();
        RefreshModifierOptions();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField(_bindingLabel, EditorStyles.boldLabel);
        using (new EditorGUI.IndentLevelScope())
        {
            EditorGUILayout.PropertyField(_actionProperty);

            GUILayout.Space(12);
            // Display the popup for binding options
            var newSelectedBinding = EditorGUILayout.Popup(_bindingLabel, _selectedBindingOption, _bindingOptions);
            if (newSelectedBinding != _selectedBindingOption)
            {
                var bindingID = _bindingOptionsValues[newSelectedBinding];
                _bindingIDProperty.stringValue = bindingID;
                _selectedBindingOption = newSelectedBinding;
            }
            EditorGUILayout.PropertyField(_bindingTextProperty);
            EditorGUILayout.PropertyField(_bindingImageProperty);


            GUILayout.Space(12);
            // Display the popup for modifier options
            var newSelectedModifier = EditorGUILayout.Popup(_modifierLabel, _selectedModifierOption, _modifierOptions);
            if (newSelectedModifier != _selectedModifierOption)
            {
                var modifierID = _modifierOptionsValues[newSelectedModifier];
                _modifierIDProperty.stringValue = modifierID;
                _selectedModifierOption = newSelectedModifier;
            }
            EditorGUILayout.PropertyField(_modifierTextProperty);
            EditorGUILayout.PropertyField(_modifierImageProperty);

            GUILayout.Space(12);
            EditorGUILayout.PropertyField(_imageParentProperty);
            EditorGUILayout.PropertyField(_textParentProperty);
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            RefreshBindingOptions();
            RefreshModifierOptions();
        }
    }

    protected void RefreshBindingOptions()
    {
        var actionReference = (InputActionReference)_actionProperty.objectReferenceValue;
        var action = actionReference?.action;

        if (action == null)
        {
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

            var displayOptions = InputBinding.DisplayStringOptions.DontUseShortDisplayNames | InputBinding.DisplayStringOptions.IgnoreBindingOverrides;
            if (!haveBindingsGroups)
            {
                displayOptions |= InputBinding.DisplayStringOptions.DontOmitDevice;
            }

            var displayString = action.GetBindingDisplayString(i, displayOptions);
            if (binding.isPartOfComposite)
            {
                displayString = $"{ObjectNames.NicifyVariableName(binding.name)} : {displayString}";
            }

            displayString = displayString.Replace('/', '\\');

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

    protected void RefreshModifierOptions()
    {
        var actionReference = (InputActionReference)_actionProperty.objectReferenceValue;
        var action = actionReference?.action;

        if (action == null)
        {
            _modifierOptions = new GUIContent[0];
            _modifierOptionsValues = new string[0];
            _selectedModifierOption = -1;
            return;
        }

        var modifiers = action.bindings;
        var modifierCount = modifiers.Count;

        _modifierOptions = new GUIContent[modifierCount];
        _modifierOptionsValues = new string[modifierCount];
        _selectedModifierOption = -1;

        var currentModifierId = _modifierIDProperty.stringValue;
        for (int i = 0; i < modifierCount; i++)
        {
            var modifier = modifiers[i];
            var modifierID = modifier.id.ToString();
            var haveModifiersGroups = !string.IsNullOrEmpty(modifier.groups);

            var displayOptions = InputBinding.DisplayStringOptions.DontUseShortDisplayNames | InputBinding.DisplayStringOptions.IgnoreBindingOverrides;
            if (!haveModifiersGroups)
            {
                displayOptions |= InputBinding.DisplayStringOptions.DontOmitDevice;
            }

            var displayString = action.GetBindingDisplayString(i, displayOptions);
            if (modifier.isPartOfComposite)
            {
                displayString = $"{ObjectNames.NicifyVariableName(modifier.name)} : {displayString}";
            }

            displayString = displayString.Replace('/', '\\');

            if (haveModifiersGroups)
            {
                var asset = action.actionMap?.asset;
                if (asset != null)
                {
                    var controlSchemes = string.Join(", ", modifier.groups.Split(InputBinding.Separator).Select(g => asset.controlSchemes.FirstOrDefault(s => s.bindingGroup == g).name));
                    displayString = $"{displayString} ({controlSchemes})";
                }
            }

            _modifierOptions[i] = new GUIContent(displayString);
            _modifierOptionsValues[i] = modifierID;

            if (currentModifierId == modifierID)
            {
                _selectedModifierOption = i;
            }
        }
    }
}
#endif
