using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RB_OptionSelectable : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler, ISelectHandler, IDeselectHandler{
    Selectable _selectable;
    bool _isSelected;

    private void Awake() {
        _selectable = GetComponent<Selectable>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (_selectable.enabled) {
            RB_OptionsSelectableManager.Instance.SelectableHooveredCount++;
            _selectable.Select();
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        RB_OptionsSelectableManager.Instance.SelectableHooveredCount--;
    }

    public void OnSelect(BaseEventData eventData) {
        RB_OptionsSelectableManager.Instance.CurrentSelectable = _selectable;
        _isSelected = true;
    }


    public void OnDeselect(BaseEventData eventData){
        _isSelected = false;

    }

    private void Update()  {
        if (!(_isSelected || (RB_OptionsSelectableManager.Instance.IsSelectableHoovered && _selectable == RB_OptionsSelectableManager.Instance.CurrentSelectable))) {
            _selectable.interactable = false;
            _selectable.interactable = true;
        }
    }
 }

