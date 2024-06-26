using MANAGERS;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RB_MainMenuButton : MonoBehaviour , IPointerEnterHandler,IPointerExitHandler, ISelectHandler, IDeselectHandler{
    [SerializeField] RB_MainMenuButtonManager.BUTTONS _currentButton;
    Button _button;
    bool _isSelected;
    float _originalXPos;

    [SerializeField] RectTransform _textTrasform;

    [SerializeField] float _offsetHover;
    [SerializeField] float _offsetSpeed;

    Selectable _oldUp;
    Selectable _oldDown;

    [SerializeField] Color _defaultColor;
    [SerializeField] Color _UnEnabledColor;

    TextMeshProUGUI _text;
    Image _buttonImage;

    Coroutine _cameraShake;

    private void Awake() {
        _originalXPos = _textTrasform.localPosition.x;
        _button = GetComponent<Button>();
        _oldUp = _button.navigation.selectOnUp.gameObject.GetComponent<Selectable>();
        _oldDown = _button.navigation.selectOnDown.gameObject.GetComponent<Selectable>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _buttonImage = GetComponent<Image>();
        _button.onClick.AddListener(OnClick);
    }

    public void OnClick() {
        RB_AudioManager.Instance.PlaySFX("click", false, false, 0, 1f);

    }

    private void Start()
    {
        StartCoroutine(LateStartCoroutine());
    }

    public IEnumerator LateStartCoroutine() {
        yield return new WaitForEndOfFrame();
        LateStart();
    }

    private void LateStart()
    {
        FixNavigation();
        RB_AudioManager.Instance.PlayMusic("Main_Menu_Music");
    }


    public void OnPointerEnter(PointerEventData eventData){
        if (_button.enabled) {
            RB_AudioManager.Instance.PlaySFX("select", false, false, 0, 1f);
            RB_MainMenuButtonManager.Instance.ButtonHooveredCount++;
            _button.Select();
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        RB_MainMenuButtonManager.Instance.ButtonHooveredCount--;
    }

    public void OnSelect(BaseEventData eventData){
        RB_MainMenuButtonManager.Instance.CurrentButton = _currentButton;
        RB_AudioManager.Instance.PlaySFX("select", false, false, 0, 1f);
        _isSelected = true;
        if (RB_SaveManager.Instance.SaveObject.IsGameFinish && RB_MainMenuButtonManager.BUTTONS.Continue == _currentButton) {
            Debug.LogWarning(name);
            _cameraShake = StartCoroutine(CameraShake.Instance.Shake(100f, 20f));
        }
     }

    public void FixNavigation() {
        Navigation buttonNavigation = _button.navigation;


        if (!_oldUp.enabled){
            buttonNavigation.selectOnUp = _oldUp.navigation.selectOnUp.gameObject.GetComponent<Button>();
            _button.navigation = buttonNavigation;
        }
        else{
            buttonNavigation.selectOnUp = _oldUp;
            _button.navigation = buttonNavigation;
        }
        if (!_oldDown.enabled) {
            buttonNavigation.selectOnDown = _oldDown.navigation.selectOnDown.gameObject.GetComponent<Button>();
            _button.navigation = buttonNavigation;
        }
        else {
            buttonNavigation.selectOnDown = _oldDown;
            _button.navigation = buttonNavigation;
        }
    }

    public void OnDeselect(BaseEventData eventData){
        _isSelected = false;
        StopCoroutine(_cameraShake);

    }

    private void Update() {
        if (!RB_MenuManager.Instance.IsOptionOpen){
            //move the text when selected or hoovered
            if ( _isSelected || (RB_MainMenuButtonManager.Instance.IsButtonsHoovered && _currentButton == RB_MainMenuButtonManager.Instance.CurrentButton)) {
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

            if ( RB_SaveManager.Instance.SaveObject.CurrentLevel < 3 && RB_MainMenuButtonManager.BUTTONS.Continue == _currentButton) {
                _button.enabled = false;
                _text.color = _UnEnabledColor;
                _buttonImage.raycastTarget = false;
                if (RB_SaveManager.Instance.SaveObject.IsGameFinish && RB_MainMenuButtonManager.BUTTONS.Continue == _currentButton){
                    _text.text = "Boss Rush";
                    _text.color = Color.red;
                    _button.enabled = true;
                    _buttonImage.raycastTarget = true;
                }
            }
            else { 
                _button.enabled = true;
                _text.color = _defaultColor;
                _buttonImage.raycastTarget = true;

            }

        }

    }


}