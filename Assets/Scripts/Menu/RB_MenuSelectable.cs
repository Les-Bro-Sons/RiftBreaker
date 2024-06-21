using MANAGERS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RB_MenuSelectable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,ISelectHandler, IDeselectHandler{
    Image _renderer;

    [SerializeField] Sprite _default;
    [SerializeField] Sprite _hoovered; //sprite when the selectable is hoovered

    [Header("Frame")]
    [SerializeField] bool _isFramed;
    [SerializeField] Image _frame;
    [SerializeField] Sprite _frameDefault;
    [SerializeField] Sprite _frameHoovered;


    bool _isHoovered;
    bool _isSelectedByNavigation;

    private void Awake(){
        _renderer = GetComponent<Image>();
        _renderer.sprite = _default;
        if(gameObject.TryGetComponent( out Button button)) {
            button.onClick.AddListener(OnClick);
        }
    }

    public void OnClick() {
        RB_AudioManager.Instance.PlaySFX("click", transform, false, 0.3f, 10f);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        _isHoovered = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        _isHoovered = false;
    }

    public void OnSelect(BaseEventData eventData){
        _isSelectedByNavigation = true;
        RB_AudioManager.Instance.PlaySFX("select" ,transform, false, 0.3f, 10f);
    }

    public void OnDeselect(BaseEventData eventData){
        _isSelectedByNavigation = false;
    }

    private void Update() {
        if (_isSelectedByNavigation || _isHoovered) {
            _renderer.sprite = _hoovered;
            if(_isFramed) { _frame.sprite = _frameHoovered; }
            
        } else {
            _renderer.sprite = _default;
            if (_isFramed) { _frame.sprite = _frameDefault; }
        }
    }
}
