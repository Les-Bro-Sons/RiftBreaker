using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RB_OptionsButton : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler, ISelectHandler, IDeselectHandler{
    [SerializeField] RB_OptionsButtonManager.OPTIONBUTTONS   _currentButton;
    Button _button;
    bool _isSelected;

    private void Awake() {
        _button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (_button.enabled) {
            RB_OptionsButtonManager.Instance.ButtonHooveredCount++;
            _button.Select();
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        RB_OptionsButtonManager.Instance.ButtonHooveredCount--;
    }

    public void OnSelect(BaseEventData eventData) {
        RB_OptionsButtonManager.Instance.CurrentButton = _currentButton;
        _isSelected = true;
    }


    public void OnDeselect(BaseEventData eventData){
        _isSelected = false;

    }

    private void Update()  {
        if (!(_isSelected || (RB_OptionsButtonManager.Instance.IsButtonsHoovered && _currentButton == RB_OptionsButtonManager.Instance.CurrentButton))) {
            _button.interactable = false;
            _button.interactable = true;
        }
    }
 }

