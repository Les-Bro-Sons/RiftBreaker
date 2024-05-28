using UnityEngine;
using UnityEngine.EventSystems;

public class RB_MenuButton : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler{

    bool _isHoovered;
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

    private void Update() {
        if (!RB_MenuManager.Instance.IsOptionOpen){
            if (_isHoovered) {
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
