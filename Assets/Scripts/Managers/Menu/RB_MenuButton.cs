using UnityEngine;
using UnityEngine.EventSystems;

public class RB_MenuButton : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler{

    bool _isHoovered;
    float _originalXPos;

    [SerializeField] Transform _textTrasform;

    [SerializeField] float _offsetHover;
    [SerializeField] float _offsetSpeed;

    private void Awake() {
        _originalXPos = _textTrasform.position.x;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        _isHoovered = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        _isHoovered = false;
    }

    private void Update() {
        if(_isHoovered) {
            float xPos = _textTrasform.position.x;
            xPos = Mathf.Lerp(xPos, _originalXPos - _offsetHover, _offsetSpeed * Time.deltaTime);
            _textTrasform.position = new Vector3(xPos, _textTrasform.position.y, _textTrasform.position.z);
        }
        else {
            float xPos = _textTrasform.position.x;
            xPos = Mathf.Lerp(xPos, _originalXPos, _offsetSpeed * Time.deltaTime);
            _textTrasform.position = new Vector3(xPos, _textTrasform.position.y, _textTrasform.position.z);
        }
    }

}
