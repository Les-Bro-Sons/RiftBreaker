using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RB_MenuSlider : MonoBehaviour , ISelectHandler, IDeselectHandler{
    Slider _slider;
    bool _isSelected;
    bool _isInteracting;

    [SerializeField] int _offsetValue = 10;
    void Start () { 
        _slider = GetComponent<Slider>();
        RB_MenuInputManager.Instance.EventSubmitStarted.AddListener(SelectSlider);
        RB_MenuInputManager.Instance.EventCancelStarted.AddListener(UnselectSlider);


    }

    public void UpdateSlider() {
        int value = Mathf.RoundToInt(_slider.value*100);

        if(RB_MenuInputManager.Instance.NavigationValue.x > 0) {
            value += _offsetValue;
        }
        else if(RB_MenuInputManager.Instance.NavigationValue.x < 0){
            value -= _offsetValue;
        }
        _slider.value = (float) value / 100;
    }



    public void SelectSlider () {
        if (_isSelected) {
            _isInteracting = true;
            _slider.interactable = false;
            RB_MenuInputManager.Instance.EventNavigateStarted.AddListener(UpdateSlider);
        }
        else if (_isInteracting) { UnselectSlider(); }
    } 

    public void UnselectSlider () {
        if (_isInteracting) {
            _isInteracting = false;
            _slider.interactable = true;
            _slider.Select();
            RB_MenuInputManager.Instance.EventNavigateStarted.RemoveListener(UpdateSlider);
        }
    }

    public void OnSelect(BaseEventData eventData){
        _isSelected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _isSelected = false;
    }
}
