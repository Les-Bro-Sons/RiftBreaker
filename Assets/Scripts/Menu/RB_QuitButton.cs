using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RB_QuitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler {
    Button _button;
    bool _isSelected;

    private void Awake() {
        _button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_button.enabled)
        {
            RB_QuitButtonManager.Instance.ButtonHooveredCount++;
            _button.Select();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RB_QuitButtonManager.Instance.ButtonHooveredCount--;
    }

    public void OnSelect(BaseEventData eventData)
    {
        RB_QuitButtonManager.Instance.CurrentButton = _button;
        _isSelected = true;
    }


    public void OnDeselect(BaseEventData eventData)
    {
        _isSelected = false;

    }

    private void Update()
    {
        if (!(_isSelected || (RB_OptionsSelectableManager.Instance.IsSelectableHoovered && _button == RB_OptionsSelectableManager.Instance.CurrentSelectable)))
        {
            _button.interactable = false;
            _button.interactable = true;
        }
    }
}

