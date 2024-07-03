using MANAGERS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;
using Cinemachine.Editor;

public class RB_SelectableTexture : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    Image _renderer; // Image component to change sprites

    [SerializeField] Sprite _defaultSprite; // Default sprite when not hovered or selected
    [SerializeField] Sprite _hooveredSprite; // Sprite when the selectable is hovered

    [SerializeField] bool _isFramed; // Toggle to enable framing
    [SerializeField] Image _frame; // Image component for the frame
    [SerializeField] Sprite _frameDefaultSprite; // Default sprite for the frame
    [SerializeField] Sprite _frameHooveredSprite; // Sprite for the frame when hovered

    bool _isHoovered; // Flag to track if the selectable is hovered
    bool _isSelectedByNavigation; // Flag to track if the selectable is selected via navigation

    private void Awake()
    {
        _renderer = GetComponent<Image>(); // Get the Image component on the same GameObject
        _renderer.sprite = _defaultSprite; // Set the default sprite initially

        // Check if this GameObject has a Button component and attach an onClick event listener
        if (gameObject.TryGetComponent(out Button button))
        {
            button.onClick.AddListener(OnClick); // Call OnClick method when the button is clicked
        }
    }

    // Method to play a click sound effect
    public void OnClick()
    {
        RB_AudioManager.Instance.PlaySFX("click", false, false, 0.3f, 10f); // Play click sound effect using AudioManager
    }

    // Interface method for when pointer enters the selectable
    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHoovered = true; // Set flag to true when hovered
    }

    // Interface method for when pointer exits the selectable
    public void OnPointerExit(PointerEventData eventData)
    {
        _isHoovered = false; // Set flag to false when not hovered
    }

    // Interface method for when the selectable is selected
    public void OnSelect(BaseEventData eventData)
    {
        RB_VibrationManager.Instance.GamepadShake(1, 1f);
        _isSelectedByNavigation = true; // Set flag to true when selected
        RB_AudioManager.Instance.PlaySFX("select", false, false, 0.3f, 10f); // Play select sound effect using AudioManager
    }

    // Interface method for when the selectable is deselected
    public void OnDeselect(BaseEventData eventData)
    {
        _isSelectedByNavigation = false; // Set flag to false when deselected
    }

    private void Update()
    {
        // Update the visual state based on whether the selectable is hovered or selected via navigation
        if (_isSelectedByNavigation || _isHoovered)
        {
            _renderer.sprite = _hooveredSprite; // Set hovered sprite
            if (_isFramed)
            {
                _frame.sprite = _frameHooveredSprite; // Set frame hovered sprite if framing is enabled
            }
        }
        else
        {
            _renderer.sprite = _defaultSprite; // Set default sprite when not hovered or selected
            if (_isFramed)
            {
                _frame.sprite = _frameDefaultSprite; // Set default frame sprite if framing is enabled
            }
        }
    }
}


#if UNITY_EDITOR

[CustomEditor(typeof(RB_SelectableTexture))]
public class RB_SelectableTextureEditor : Editor
{
    #region Serialized Properties
    SerializedProperty _defaultSpriteProperty;
    SerializedProperty _hooveredSpriteProperty;

    SerializedProperty _isFramedProperty;
    SerializedProperty _frameProperty;
    SerializedProperty _frameDefaultSpriteProperty;
    SerializedProperty _frameHooveredSpriteProperty;

    GUIContent _imageLabel = new GUIContent("Image");
    GUIContent _frameLabel = new GUIContent("Frame");
    #endregion

    private void OnEnable()
    {
        _defaultSpriteProperty = serializedObject.FindProperty("_defaultSprite");
        _hooveredSpriteProperty = serializedObject.FindProperty("_hooveredSprite");

        _isFramedProperty = serializedObject.FindProperty("_isFramed");
        _frameProperty = serializedObject.FindProperty("_frame");
        _frameDefaultSpriteProperty = serializedObject.FindProperty("_frameDefaultSprite");
        _frameHooveredSpriteProperty = serializedObject.FindProperty("_frameHooveredSprite");
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized object
        serializedObject.Update();

        // Draw the default sprite property
        EditorGUILayout.LabelField(_imageLabel, EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_defaultSpriteProperty);
        EditorGUILayout.PropertyField(_hooveredSpriteProperty);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField(_frameLabel, EditorStyles.boldLabel);
        // Draw the framed property
        EditorGUILayout.PropertyField(_isFramedProperty);

        // Conditionally draw frame properties
        if (_isFramedProperty.boolValue)
        {
            EditorGUILayout.PropertyField(_frameProperty);
            EditorGUILayout.PropertyField(_frameDefaultSpriteProperty);
            EditorGUILayout.PropertyField(_frameHooveredSpriteProperty);
        }

        // Apply the changes to the serialized object
        serializedObject.ApplyModifiedProperties();
    }
}

#endif
