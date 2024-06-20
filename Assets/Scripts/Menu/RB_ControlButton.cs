using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static RB_MenuControlManager;

public class RB_ControlButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler{
    Image _renderer;
    [Header("Icon")]
    [SerializeField] Image _iconRenderer;
    [SerializeField] Material _iconMaterial;

    [Header("Button")]
    [SerializeField]BINDERS _binder;
    bool _isHoovered;
    bool _isSelectedByNavigation;

    [SerializeField] Sprite _lighter; //sprite when the button is not actually selected and hoovered
    [SerializeField] Sprite _hoovered; //sprite when the button is hoovered
    [SerializeField] Sprite _selected; //sprite when the button is selected
    [SerializeField] Sprite _hooveredSelected; //sprite when the button is hoovered and selected

    private void Start(){
        _renderer = GetComponent<Image>();
    }

    private void Update() {
        if (_isSelectedByNavigation || _isHoovered) {
            if(_binder == RB_MenuControlManager.Instance.CurrentBinder) {
                _renderer.sprite = _hooveredSelected;
                _iconRenderer.material = _iconMaterial;
            }
            else {
                _renderer.sprite = _hoovered;
                _iconRenderer.material = null;
            }
        } else {
            if (_binder == RB_MenuControlManager.Instance.CurrentBinder){
                _renderer.sprite = _selected;
                _iconRenderer.material = _iconMaterial;
            }
            else {
                _renderer.sprite = _lighter;
                _iconRenderer.material = null;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        _isHoovered = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        _isHoovered = false;
    }

    public void OnSelect(BaseEventData eventData) {
        _isSelectedByNavigation = true;
    }

    public void OnDeselect(BaseEventData eventData) {
        _isSelectedByNavigation = false;
    }
}
