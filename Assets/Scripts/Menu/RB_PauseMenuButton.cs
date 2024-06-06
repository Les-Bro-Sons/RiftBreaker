using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RB_PauseMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler{
    [SerializeField] RB_MainMenuButtonManager.PAUSE_BUTTONS currentButton;
    Button _button;
    bool _isSelected;

    [SerializeField] Sprite _default;
    [SerializeField] Sprite _hoovered;
    Image _renderer;

    private void Awake() {
        _button = GetComponent<Button>();
        _renderer = GetComponent<Image>();
        _renderer.sprite = _default;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        RB_MainMenuButtonManager.Instance.ButtonHooveredCount++;
        _button.Select();
    }

    public void OnPointerExit(PointerEventData eventData) {
        RB_MainMenuButtonManager.Instance.ButtonHooveredCount--;
    }

    public void OnSelect(BaseEventData eventData) {
        RB_MainMenuButtonManager.Instance.CurrentButton2 = currentButton;
        _isSelected = true;
    }

    public void OnDeselect(BaseEventData eventData){ 
        _isSelected = false;
    }

     private void Update() {
        if (!RB_MenuManager.Instance.IsOptionOpen){
            //move the text when selected or hoovered
            if ( _isSelected || (RB_MainMenuButtonManager.Instance.IsButtonsHoovered && currentButton == RB_MainMenuButtonManager.Instance.CurrentButton2)) {
                _renderer.sprite = _hoovered;
            }
            else {
                _button.interactable = false;
                _button.interactable = true;
                _renderer.sprite = _default;
            }
        }

    }
}
