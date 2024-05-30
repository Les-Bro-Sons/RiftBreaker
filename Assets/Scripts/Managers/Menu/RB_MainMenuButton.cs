using UnityEngine;
using UnityEngine.EventSystems;

public class RB_MainMenuButton : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler{

    bool _isHoovered;
    bool _isSelected;
    float _originalXPos;

    [SerializeField] RectTransform _textTrasform;

    [SerializeField] float _offsetHover;
    [SerializeField] float _offsetSpeed;

    private void Awake() {
        _originalXPos = _textTrasform.localPosition.x;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        _isHoovered = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        _isHoovered = false;
    }

    public void OnSelect(BaseEventData eventData){
        _isSelected = true;
    }

    public void OnDeselect(BaseEventData eventData){
        _isSelected = false;
    }

    private void Update() {
        if (!RB_MenuManager.Instance.IsOptionOpen){
            if (_isHoovered || _isSelected) {
                float xPos = _textTrasform.localPosition.x;
                xPos = Mathf.Lerp(xPos, _originalXPos - _offsetHover, _offsetSpeed * Time.deltaTime);
                _textTrasform.localPosition = new Vector3(xPos, _textTrasform.localPosition.y, _textTrasform.localPosition.z);
            }
            else {
                float xPos = _textTrasform.localPosition.x;
                xPos = Mathf.Lerp(xPos, _originalXPos, _offsetSpeed * Time.deltaTime);
                _textTrasform.localPosition = new Vector3(xPos, _textTrasform.localPosition.y, _textTrasform.localPosition.z);
            }
        }

    }


}
