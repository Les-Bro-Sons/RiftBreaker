using MANAGERS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RB_SelectableTexture : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    Image _renderer; // Image component to change sprites

    [SerializeField] Sprite _default; // Default sprite when not hovered or selected
    [SerializeField] Sprite _hoovered; // Sprite when the selectable is hovered

    [Header("Frame")]
    [SerializeField] bool _isFramed; // Toggle to enable framing
    [SerializeField] Image _frame; // Image component for the frame
    [SerializeField] Sprite _frameDefault; // Default sprite for the frame
    [SerializeField] Sprite _frameHoovered; // Sprite for the frame when hovered

    bool _isHoovered; // Flag to track if the selectable is hovered
    bool _isSelectedByNavigation; // Flag to track if the selectable is selected via navigation

    private void Awake()
    {
        _renderer = GetComponent<Image>(); // Get the Image component on the same GameObject
        _renderer.sprite = _default; // Set the default sprite initially

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
            _renderer.sprite = _hoovered; // Set hovered sprite
            if (_isFramed)
            {
                _frame.sprite = _frameHoovered; // Set frame hovered sprite if framing is enabled
            }
        }
        else
        {
            _renderer.sprite = _default; // Set default sprite when not hovered or selected
            if (_isFramed)
            {
                _frame.sprite = _frameDefault; // Set default frame sprite if framing is enabled
            }
        }
    }
}
