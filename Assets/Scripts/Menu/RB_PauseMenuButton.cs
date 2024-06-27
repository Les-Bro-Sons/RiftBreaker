using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RB_PauseMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] RB_MainMenuButtonManager.PAUSE_BUTTONS currentButton; // Enum field to represent the current button type
    Button _button;      // Reference to the Button component attached to this GameObject
    bool _isSelected;    // Flag indicating whether this button is currently selected

    [SerializeField] Sprite _default;   // Default sprite for the button
    [SerializeField] Sprite _hoovered;  // Sprite to show when the button is hovered over
    Image _renderer;    // Reference to the Image component attached to this GameObject

    private void Awake()
    {
        _button = GetComponent<Button>();   // Get the Button component on Awake
        _renderer = GetComponent<Image>();  // Get the Image component on Awake
        _renderer.sprite = _default;    // Set the default sprite initially
    }

    // Called when pointer enters the button area
    public void OnPointerEnter(PointerEventData eventData)
    {
        RB_MainMenuButtonManager.Instance.ButtonHooveredCount++;  // Increment the hoovered button count in the manager
        _button.Select();   // Select this button
    }

    // Called when pointer exits the button area
    public void OnPointerExit(PointerEventData eventData)
    {
        RB_MainMenuButtonManager.Instance.ButtonHooveredCount--;  // Decrement the hoovered button count in the manager
    }

    // Called when the button is selected
    public void OnSelect(BaseEventData eventData)
    {
        RB_MainMenuButtonManager.Instance.CurrentButton2 = currentButton;  // Set this button type as the current button in the manager
        _isSelected = true;   // Mark this button as selected
    }

    // Called when the button is deselected
    public void OnDeselect(BaseEventData eventData)
    {
        _isSelected = false;  // Mark this button as deselected
    }

    private void Update()
    {
        // Check if the option menu is not open
        if (!RB_MenuManager.Instance.IsOptionOpen)
        {
            // Check conditions to update button appearance and interactability
            if (_isSelected || (RB_MainMenuButtonManager.Instance.IsButtonsHoovered && currentButton == RB_MainMenuButtonManager.Instance.CurrentButton2))
            {
                _renderer.sprite = _hoovered;   // Change to hovered sprite if selected or hovered
            }
            else
            {
                _button.interactable = false;   // Disable interactability temporarily
                _button.interactable = true;    // Enable interactability (resetting it)
                _renderer.sprite = _default;    // Set back to default sprite
            }
        }
    }
}
