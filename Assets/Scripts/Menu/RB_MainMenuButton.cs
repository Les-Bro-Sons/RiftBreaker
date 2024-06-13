using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RB_MainMenuButton : MonoBehaviour , IPointerEnterHandler,IPointerExitHandler, ISelectHandler, IDeselectHandler{
    [SerializeField] RB_MainMenuButtonManager.BUTTONS currentButton;
    Button _button;
    bool _isSelected;
    float _originalXPos;

    [SerializeField] RectTransform _textTrasform;

    [SerializeField] float _offsetHover;
    [SerializeField] float _offsetSpeed;

    Selectable _oldUp;
    Selectable _oldDown;

    private void Awake() {
        _originalXPos = _textTrasform.localPosition.x;
        _button = GetComponent<Button>();
        _oldUp = _button.navigation.selectOnUp.gameObject.GetComponent<Selectable>();
        _oldDown = _button.navigation.selectOnDown.gameObject.GetComponent<Selectable>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        RB_MainMenuButtonManager.Instance.ButtonHooveredCount++;
        _button.Select();
    }

    public void OnPointerExit(PointerEventData eventData) {
        RB_MainMenuButtonManager.Instance.ButtonHooveredCount--;
    }

    public void OnSelect(BaseEventData eventData){
        RB_MainMenuButtonManager.Instance.CurrentButton = currentButton;
        _isSelected = true;

        Navigation buttonNavigation = _button.navigation;
        if (!buttonNavigation.selectOnUp.gameObject.GetComponent<Button>().enabled) {
            buttonNavigation.selectOnUp = _button.navigation.selectOnUp.navigation.selectOnUp.gameObject.GetComponent<Button>();
            _button.navigation = buttonNavigation;
        }
        else {
            buttonNavigation.selectOnUp = _oldUp;
            _button.navigation = buttonNavigation;
        }
        if (!buttonNavigation.selectOnDown.gameObject.GetComponent<Button>().enabled) {
            buttonNavigation.selectOnDown = _button.navigation.selectOnDown.navigation.selectOnDown.gameObject.GetComponent<Button>();
            _button.navigation = buttonNavigation;
        }
        else {
            buttonNavigation.selectOnDown = _oldDown;
            _button.navigation = buttonNavigation;
        }
    }

    public void OnDeselect(BaseEventData eventData){
        _isSelected = false;
        
    }

    private void Update() {
        if (!RB_MenuManager.Instance.IsOptionOpen){
            //move the text when selected or hoovered
            if ( _isSelected || (RB_MainMenuButtonManager.Instance.IsButtonsHoovered && currentButton == RB_MainMenuButtonManager.Instance.CurrentButton)) {
                float xPos = _textTrasform.localPosition.x;
                xPos = Mathf.Lerp(xPos, _originalXPos - _offsetHover, _offsetSpeed * Time.deltaTime);
                _textTrasform.localPosition = new Vector3(xPos, _textTrasform.localPosition.y, _textTrasform.localPosition.z);
            }
            else {
                _button.interactable = false;
                _button.interactable = true;
                float xPos = _textTrasform.localPosition.x;
                xPos = Mathf.Lerp(xPos, _originalXPos, _offsetSpeed * Time.deltaTime);
                _textTrasform.localPosition = new Vector3(xPos, _textTrasform.localPosition.y, _textTrasform.localPosition.z);
            }
        }

    }


}