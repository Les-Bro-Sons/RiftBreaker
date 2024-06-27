using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RB_OptionSelectable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    Selectable _selectable;  // Reference to the Selectable component attached to this GameObject
    bool _isSelected;        // Flag indicating whether this selectable is currently selected

    private void Awake()
    {
        _selectable = GetComponent<Selectable>();  // Get the Selectable component on Awake
    }

    // Called when pointer enters the selectable area
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_selectable.enabled)
        {  // Check if the Selectable component is enabled
            RB_OptionsSelectableManager.Instance.SelectableHooveredCount++;  // Increment the hoovered count in RB_OptionsSelectableManager
            _selectable.Select();  // Select this selectable
        }
    }

    // Called when pointer exits the selectable area
    public void OnPointerExit(PointerEventData eventData)
    {
        RB_OptionsSelectableManager.Instance.SelectableHooveredCount--;  // Decrement the hoovered count in RB_OptionsSelectableManager
    }

    // Called when the selectable is selected
    public void OnSelect(BaseEventData eventData)
    {
        RB_OptionsSelectableManager.Instance.CurrentSelectable = _selectable;  // Set this selectable as the current selectable in RB_OptionsSelectableManager
        _isSelected = true;  // Mark this selectable as selected
    }

    // Called when the selectable is deselected
    public void OnDeselect(BaseEventData eventData)
    {
        _isSelected = false;  // Mark this selectable as deselected
    }

    private void Update()
    {
        // Check if neither this selectable is selected nor any other selectable is hovered over and selected
        if (!(_isSelected || (RB_OptionsSelectableManager.Instance.IsSelectableHoovered && _selectable == RB_OptionsSelectableManager.Instance.CurrentSelectable)))
        {
            _selectable.interactable = false;  // Temporarily disable interactability
            _selectable.interactable = true;   // Enable interactability (resetting it)
        }
    }
}
