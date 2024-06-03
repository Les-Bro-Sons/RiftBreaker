using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RB_MenuExitScrollBar : MonoBehaviour, IPointerExitHandler, ISelectHandler, IPointerDownHandler{
    [SerializeField] Selectable _nextSelectable;
    [SerializeField] bool _isSelected;


    Scrollbar _scrollbar;


    private void Start()
    {
        _scrollbar = GetComponent<Scrollbar>();
    }
    /*    public void OnPointerEnter(PointerEventData eventData){

        }*/

    public void OnPointerExit(PointerEventData eventData){
        if (_isSelected) {
            _scrollbar.interactable = false;
            _scrollbar.interactable = true;
            _nextSelectable.Select();
            _isSelected = false;
        }
    }

    public void OnSelect(BaseEventData eventData){
        Debug.Log("AAAA");
        _isSelected = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("AAAA");
        _isSelected = true;
    }
}
