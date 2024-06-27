using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static RB_MenuControlManager;

public class RB_ControlButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    Image _renderer; // Image component attached to this GameObject

    [Header("Icon")]
    [SerializeField] Image _iconRenderer; // Reference to the Image component for the icon
    [SerializeField] Material _iconMaterial; // Material to apply to the icon when selected

    [Header("Button")]
    [SerializeField] BINDERS _binder; // Enum to determine button state
    bool _isHoovered; // Flag indicating if the button is hovered
    bool _isSelectedByNavigation; // Flag indicating if the button is selected via navigation

    [SerializeField] Sprite _lighter; // Sprite when the button is not selected or hovered
    [SerializeField] Sprite _hoovered; // Sprite when the button is hovered
    [SerializeField] Sprite _selected; // Sprite when the button is selected
    [SerializeField] Sprite _hooveredSelected; // Sprite when the button is hovered and selected

    private void Start()
    {
        _renderer = GetComponent<Image>(); // Get the Image component attached to this GameObject
    }

    private void Update()
    {
        // Update button visuals based on current state
        if (_isSelectedByNavigation || _isHoovered)
        {
            // Button is either selected by navigation or hovered
            if (_binder == RB_MenuControlManager.Instance.CurrentBinder)
            {
                // When binder matches current binder in RB_MenuControlManager
                _renderer.sprite = _hooveredSelected; // Use hoovered and selected sprite
                _iconRenderer.material = _iconMaterial; // Apply icon material
            }
            else
            {
                // When binder does not match current binder
                _renderer.sprite = _hoovered; // Use hoovered sprite
                _iconRenderer.material = null; // Clear icon material
            }
        }
        else
        {
            // Button is neither selected by navigation nor hovered
            if (_binder == RB_MenuControlManager.Instance.CurrentBinder)
            {
                // When binder matches current binder in RB_MenuControlManager
                _renderer.sprite = _selected; // Use selected sprite
                _iconRenderer.material = _iconMaterial; // Apply icon material
            }
            else
            {
                // When binder does not match current binder
                _renderer.sprite = _lighter; // Use lighter sprite
                _iconRenderer.material = null; // Clear icon material
            }
        }
    }

    // Interface method: Called when the pointer enters the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHoovered = true; // Set hovered flag to true
    }

    // Interface method: Called when the pointer exits the button
    public void OnPointerExit(PointerEventData eventData)
    {
        _isHoovered = false; // Set hovered flag to false
    }

    // Interface method: Called when the button is selected by navigation
    public void OnSelect(BaseEventData eventData)
    {
        _isSelectedByNavigation = true; // Set selected by navigation flag to true
    }

    // Interface method: Called when the button is deselected by navigation
    public void OnDeselect(BaseEventData eventData)
    {
        _isSelectedByNavigation = false; // Set selected by navigation flag to false
    }
}
