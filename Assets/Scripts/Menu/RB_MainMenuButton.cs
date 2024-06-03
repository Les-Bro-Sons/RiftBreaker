using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RB_MainMenuButton : MonoBehaviour , IPointerEnterHandler,IPointerExitHandler, ISelectHandler, IDeselectHandler{
    [SerializeField] RB_MainMenuButtonManager.BUTTONS currentButton;
    Button _button;
    bool _isHoovered;
    bool _isSelected;
    float _originalXPos;

    [SerializeField] RectTransform _textTrasform;

    [SerializeField] float _offsetHover;
    [SerializeField] float _offsetSpeed;

    private void Awake() {
        _originalXPos = _textTrasform.localPosition.x;
        _button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        _isHoovered = true;
        RB_MainMenuButtonManager.Instance.ButtonHooveredCount++;
        _button.Select();
    }

    public void OnPointerExit(PointerEventData eventData) {
        RB_MainMenuButtonManager.Instance.ButtonHooveredCount--;
        _isHoovered = false;
    }

    public void OnSelect(BaseEventData eventData){
        RB_MainMenuButtonManager.Instance.CurrentButton = currentButton;
        _isSelected = true;
    }

    public void OnDeselect(BaseEventData eventData){
        _isSelected = false;
    }

    private void Update() {
        if (!RB_MenuManager.Instance.IsOptionOpen){
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