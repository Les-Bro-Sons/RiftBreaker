using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RB_MenuExitScrollBar : MonoBehaviour, IPointerDownHandler, IPointerUpHandler{
    [SerializeField] Selectable _nextSelectable;
    [SerializeField] bool _isSelected;

    Scrollbar _scrollbar;

    private void Start() {
        _scrollbar = GetComponent<Scrollbar>();
    }

    // Called when the pointer is released to Select the next Selectable
    public void OnPointerUp(PointerEventData eventData){
        if (_isSelected) {
            // Temporarily disable and enable the scrollbar to refresh its state
            _scrollbar.interactable = false;
            _scrollbar.interactable = true;

            _nextSelectable.Select();
            _isSelected = false;
        }
    }

    //Called when the pointer is pressed down
    public void OnPointerDown(PointerEventData eventData) {
        _isSelected = true;
    }
}
