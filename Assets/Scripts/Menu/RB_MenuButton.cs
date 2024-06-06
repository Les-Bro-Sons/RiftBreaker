using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RB_MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,ISelectHandler, IDeselectHandler{

    [SerializeField] Sprite _default;
    [SerializeField] Sprite _hoovered; //sprite when the gameobject is hoovered
    [SerializeField] Sprite _selected; //sprite when the gameobject is selected

    Image _renderer;

    private void Awake(){
        _renderer = GetComponent<Image>();
        _renderer.sprite = _default;
    }


    public void OnPointerEnter(PointerEventData eventData) {
        _renderer.sprite = _hoovered;
    }

    public void OnPointerExit(PointerEventData eventData) {
        _renderer.sprite = _default;
    }

    public void OnSelect(BaseEventData eventData){
        _renderer.sprite = _hoovered;
    }

    public void OnDeselect(BaseEventData eventData){
        _renderer.sprite = _default;
    }
}
