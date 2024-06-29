using MANAGERS; 
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RB_MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] RB_MainMenuButtonManager.BUTTONS _currentButton; // Enum to identify the current button
    Button _button; // Reference to the Button component attached to this GameObject
    bool _isSelected; // Flag indicating if the button is currently selected
    float _originalXPos; // Original X position of the button's text

    [SerializeField] RectTransform _textTrasform; // Reference to the RectTransform of the text (used for animation)

    [SerializeField] float _offsetHover; // Offset for text movement when hovered or selected
    [SerializeField] float _offsetSpeed; // Speed of text movement animation

    Selectable _oldUp; // Reference to the selectable above this button in navigation
    Selectable _oldDown; // Reference to the selectable below this button in navigation

    [SerializeField] Color _defaultColor; // Default color of the text
    [SerializeField] Color _UnEnabledColor; // Color of the text when button is disabled

    TextMeshProUGUI _text; // Reference to the TextMeshProUGUI component for the button's text
    Image _buttonImage; // Reference to the Image component of the button

    Coroutine _cameraShake; // Coroutine reference for camera shake

    private void Awake()
    {
        _originalXPos = _textTrasform.localPosition.x; // Store the original X position of the text
        _button = GetComponent<Button>(); // Get the Button component
        _oldUp = _button.navigation.selectOnUp.gameObject.GetComponent<Selectable>(); // Get the selectable above this button
        _oldDown = _button.navigation.selectOnDown.gameObject.GetComponent<Selectable>(); // Get the selectable below this button
        _text = GetComponentInChildren<TextMeshProUGUI>(); // Get the TextMeshProUGUI component for the button's text
        _buttonImage = GetComponent<Image>(); // Get the Image component of the button
        _button.onClick.AddListener(OnClick); // Add listener for button click event
    }

    public void OnClick()
    {
        RB_AudioManager.Instance.PlaySFX("click", false, false, 0, 1f); // Play click sound effect
    }

    private void Start()
    {
        StartCoroutine(LateStartCoroutine()); // Start coroutine to handle late initialization
    }

    public IEnumerator LateStartCoroutine()
    {
        yield return new WaitForEndOfFrame(); // Wait for end of frame before continuing
        LateStart(); // Call late initialization method
    }

    private void LateStart()
    {
        FixNavigation(); // Fix navigation references for the button
        RB_AudioManager.Instance.PlayMusic("Main_Menu_Music"); // Play main menu music
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_button.enabled)
        {
            RB_AudioManager.Instance.PlaySFX("select", false, false, 0, 1f); // Play select sound effect
            RB_MainMenuButtonManager.Instance.ButtonHooveredCount++; // Increase hoovered count in button manager
            _button.Select(); // Select this button
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RB_MainMenuButtonManager.Instance.ButtonHooveredCount--; // Decrease hoovered count in button manager
    }
   
    public void OnSelect(BaseEventData eventData) {
        RB_InputManager.Instance.GamepadShake(10, 30, 0.2f);
        RB_MainMenuButtonManager.Instance.CurrentButton = _currentButton; // Set current button in button manager
        RB_AudioManager.Instance.PlaySFX("select", false, false, 0, 1f); // Play select sound effect
        _isSelected = true; // Set selected flag to true
        // If game is finished and this button is Continue, start camera shake
        if (RB_SaveManager.Instance.SaveObject.IsGameFinish && RB_MainMenuButtonManager.BUTTONS.Continue == _currentButton)
        {
            _cameraShake = StartCoroutine(RB_ButtonShake.Instance.Shake(100f, 20f)); // Start camera shake coroutine
        }
    }

    public void FixNavigation()
    {
        Navigation buttonNavigation = _button.navigation; // Get current navigation settings of the button

        // Fix selectOnUp navigation
        if (!_oldUp.enabled)
        {
            buttonNavigation.selectOnUp = _oldUp.navigation.selectOnUp.gameObject.GetComponent<Button>();
            _button.navigation = buttonNavigation;
        }
        else
        {
            buttonNavigation.selectOnUp = _oldUp;
            _button.navigation = buttonNavigation;
        }

        // Fix selectOnDown navigation
        if (!_oldDown.enabled)
        {
            buttonNavigation.selectOnDown = _oldDown.navigation.selectOnDown.gameObject.GetComponent<Button>();
            _button.navigation = buttonNavigation;
        }
        else
        {
            buttonNavigation.selectOnDown = _oldDown;
            _button.navigation = buttonNavigation;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _isSelected = false; // Set selected flag to false

        // If game is finished and this button is Continue, stop camera shake
        if (RB_SaveManager.Instance.SaveObject.IsGameFinish && RB_MainMenuButtonManager.BUTTONS.Continue == _currentButton)
        {
            StopCoroutine(_cameraShake); // Stop camera shake coroutine
        }
    }

    private void Update()
    {
        // Update button visuals and interaction based on game state and button status
        if (!RB_MenuManager.Instance.IsOptionOpen)
        {
            // Move the text when selected or hovered
            if (_isSelected || (RB_MainMenuButtonManager.Instance.IsButtonsHoovered && _currentButton == RB_MainMenuButtonManager.Instance.CurrentButton))
            {
                float xPos = _textTrasform.localPosition.x;
                xPos = Mathf.Lerp(xPos, _originalXPos - _offsetHover, _offsetSpeed * Time.deltaTime);
                _textTrasform.localPosition = new Vector3(xPos, _textTrasform.localPosition.y, _textTrasform.localPosition.z);
            }
            else
            {
                // Return text to original position
                float xPos = _textTrasform.localPosition.x;
                xPos = Mathf.Lerp(xPos, _originalXPos, _offsetSpeed * Time.deltaTime);
                _textTrasform.localPosition = new Vector3(xPos, _textTrasform.localPosition.y, _textTrasform.localPosition.z);
            }

            if (RB_SaveManager.Instance.SaveObject.IsGameFinish && RB_MainMenuButtonManager.BUTTONS.Continue == _currentButton)
            {
                _text.text = "Boss Rush"; // Change button text to "Boss Rush"
                _text.color = Color.red; // Change text color to red
                _button.enabled = true; // Enable button interaction
                _buttonImage.raycastTarget = true; // Enable raycast target of the button image
            }

            // Handle button interaction based on game state and button conditions
            else if (RB_SaveManager.Instance.SaveObject.CurrentLevel < 3 && RB_MainMenuButtonManager.BUTTONS.Continue == _currentButton)
            {
                _button.enabled = false; // Disable button interaction
                _text.color = _UnEnabledColor; // Change text color to disabled color
                _buttonImage.raycastTarget = false; // Disable raycast target of the button image

                // If game is finished and this button is Continue, enable specific conditions

            }
            else
            {
                // Enable button interaction and set default text color and raycast target
                _button.enabled = true;
                _text.color = _defaultColor;
                _buttonImage.raycastTarget = true;
            }
        }
    }
}
