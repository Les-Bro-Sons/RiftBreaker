using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Ensure the GameObject has a ScrollRect component
[RequireComponent(typeof(ScrollRect))]
public class RB_MenuAutomaticScrollRect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    
    public float ScrollSpeed = 10f;
    bool _isHoovered = false;

    // List to store all selectable UI elements within the ScrollRect
    List<Selectable> _selectables = new List<Selectable>();
    ScrollRect _scrollRect;

    // Variable to store the new position of the scrollbar
    Vector2 _newScrollBarPos = Vector2.up;

    void OnEnable(){
        // Ensure the _scrollRect is initialized before accessing its content
        if (_scrollRect != null){
            // Get all selectable elements within the ScrollRect's content
            _scrollRect.content.GetComponentsInChildren(_selectables);
        }
    }

    void Awake() { 
        _scrollRect = GetComponent<ScrollRect>();
    }

    void Start() {
        // Add a listener to the navigation event to handle scrolling
        RB_MenuInputManager.Instance.EventNavigateStarted.AddListener(InputScroll);
        // Initialize the _selectables list if _scrollRect is not null
        if (_scrollRect != null) { 
            _scrollRect.content.GetComponentsInChildren(_selectables);
        }
        // Scroll to the selected item immediately
        ScrollToSelected(true);
    }

    void Update() {
        // Handle input-based scrolling
        InputScroll();
        // Smoothly scroll to the new position if the pointer is not over the scroll area
        if (!_isHoovered) { 
            _scrollRect.normalizedPosition = Vector2.Lerp(_scrollRect.normalizedPosition, _newScrollBarPos, ScrollSpeed * Time.unscaledDeltaTime);
        }
        // Update the new scroll bar position if the pointer is over the scroll area
        else {
            _newScrollBarPos = _scrollRect.normalizedPosition;
        }
    }

    // Handle input-based scrolling
    void InputScroll() { 
        if (_selectables.Count > 0) {
            ScrollToSelected(false);
        }
    }

    // Scroll to the currently selected UI element
    void ScrollToSelected(bool isQuickScroll) {
        int selectedID = -1;
        // Get the currently selected UI element
        Selectable selectedItem = EventSystem.current.currentSelectedGameObject ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null;
        if (selectedItem != null) { 
            // Find the index of the selected item in the _selectables list
            selectedID = _selectables.IndexOf(selectedItem);
        }
        if (selectedID > -1) {
            if (isQuickScroll) { 
                // Immediately set the scroll position for quick scroll
                _scrollRect.normalizedPosition = new Vector2(0, 1 - (selectedID / ((float)_selectables.Count - 1)));
                _newScrollBarPos = _scrollRect.normalizedPosition;
            } else {
                // Set the target scroll position for smooth scroll
                _newScrollBarPos = new Vector2(0, 1 - (selectedID / ((float)_selectables.Count - 1)));
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        _isHoovered = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        _isHoovered = false;
        ScrollToSelected(false);
    }
}
