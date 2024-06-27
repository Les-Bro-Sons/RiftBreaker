using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RB_MenuSlider : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    Slider _slider;                  // Reference to the Slider component attached to this GameObject
    [SerializeField] bool _isSelected;   // Flag indicating whether this slider is currently selected
    [SerializeField] bool _isInteracting; // Flag indicating whether the slider is currently being interacted with

    [SerializeField] int _offsetValue = 10;  // Offset value used for adjusting slider value on navigation

    [Header("Handler")]
    [SerializeField] GameObject _handler;   // Reference to the handler GameObject (usually a visual indicator like an image)
    Image _handlerRenderer;                 // Reference to the Image component of the handler
    [SerializeField] Sprite _default;       // Default sprite for the handler
    [SerializeField] Sprite _selected;      // Selected sprite for the handler

    void Start()
    {
        _slider = GetComponent<Slider>();  // Get the Slider component on Start
        // Subscribe to input events from RB_MenuInputManager
        RB_MenuInputManager.Instance.EventSubmitStarted.AddListener(SelectSlider);
        RB_MenuInputManager.Instance.EventCancelStarted.AddListener(UnselectSlider);
        _handlerRenderer = _handler.GetComponent<Image>();  // Get the Image component of the handler
    }

    // Update the slider value based on navigation input
    public void UpdateSlider()
    {
        int value = Mathf.RoundToInt(_slider.value * 100);

        if (RB_MenuInputManager.Instance.NavigationValue.x > 0)
        {
            value += _offsetValue;  // Increase slider value by offset if navigating right
        }
        else if (RB_MenuInputManager.Instance.NavigationValue.x < 0)
        {
            value -= _offsetValue;  // Decrease slider value by offset if navigating left
        }
        _slider.value = (float)value / 100;  // Update slider value
    }

    // Handle selection of the slider
    public void SelectSlider()
    {
        if (_isSelected)
        {
            _isInteracting = true;
            _slider.interactable = false;  // Disable slider interaction
            _handlerRenderer.sprite = _selected;  // Change handler sprite to selected state
            RB_MenuInputManager.Instance.EventNavigateStarted.AddListener(UpdateSlider);  // Listen for navigation input to update slider
        }
        else if (_isInteracting)
        {
            UnselectSlider();  // If already interacting, unselect the slider
        }
    }

    // Handle unselection of the slider
    public void UnselectSlider()
    {
        if (_isInteracting)
        {
            _isInteracting = false;
            _slider.interactable = true;  // Enable slider interaction
            _slider.Select();  // Select the slider
            RB_MenuInputManager.Instance.EventNavigateStarted.RemoveListener(UpdateSlider);  // Stop listening for navigation input
        }
    }

    // Reset interaction state of the slider
    public void ResetInteraction()
    {
        if (_isInteracting)
        {
            _isInteracting = false;
            _slider.interactable = true;  // Enable slider interaction
            _handlerRenderer.sprite = _default;  // Reset handler sprite to default
            _isSelected = false;
            RB_MenuInputManager.Instance.EventNavigateStarted.RemoveListener(UpdateSlider);  // Stop listening for navigation input
        }
    }

    // Called when the slider is selected
    public void OnSelect(BaseEventData eventData)
    {
        _handlerRenderer.sprite = _selected;  // Change handler sprite to selected state
        _isSelected = true;
    }

    // Called when the slider is deselected
    public void OnDeselect(BaseEventData eventData)
    {
        _isSelected = false;
        _handlerRenderer.sprite = _default;  // Reset handler sprite to default
    }
}
