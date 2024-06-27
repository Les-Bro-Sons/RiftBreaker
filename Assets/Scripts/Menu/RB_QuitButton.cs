using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RB_QuitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    Button _button;      // Reference to the Button component attached to this GameObject
    bool _isSelected;    // Flag indicating whether this button is currently selected

    private void Awake()
    {
        _button = GetComponent<Button>();   // Get the Button component on Awake
    }

    // Called when pointer enters the button area
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_button.enabled)
        {  // Check if the button is enabled
            RB_QuitButtonManager.Instance.ButtonHooveredCount++;  // Increment the hoovered button count in the manager
            _button.Select();   // Select this button
        }
    }

    // Called when pointer exits the button area
    public void OnPointerExit(PointerEventData eventData)
    {
        RB_QuitButtonManager.Instance.ButtonHooveredCount--;  // Decrement the hoovered button count in the manager
    }

    // Called when the button is selected
    public void OnSelect(BaseEventData eventData)
    {
        RB_QuitButtonManager.Instance.CurrentButton = _button;  // Set this button as the current button in the manager
        _isSelected = true;   // Mark this button as selected
    }

    // Called when the button is deselected
    public void OnDeselect(BaseEventData eventData)
    {
        _isSelected = false;  // Mark this button as deselected
    }

    private void Update()
    {
        // Check conditions to enable/disable the button interactability
        if (!(_isSelected || (RB_OptionsSelectableManager.Instance.IsSelectableHoovered && _button == RB_OptionsSelectableManager.Instance.CurrentSelectable)))
        {
            _button.interactable = false;   // Disable interactability
            _button.interactable = true;    // Enable interactability (resetting it)
        }
    }
}
