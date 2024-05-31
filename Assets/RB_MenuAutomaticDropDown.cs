using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class RB_MenuAutomaticDropDown : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{
    public float ScrollSpeed = 10f;
    bool _isHoovered = false;

    List<Selectable> _selectables = new List<Selectable>();
    ScrollRect _scrollRect;

    Vector2 _newScrollBarPos = Vector2.up;


    void OnEnable(){
        if (_scrollRect != null){
            _scrollRect.content.GetComponentsInChildren(_selectables);
        }
    }

    void Awake() { 
        _scrollRect = GetComponent<ScrollRect>();
    }

    void Start() {
        RB_MenuInputManager.Instance.EventNavigateStarted.AddListener(InputScroll);
        if (_scrollRect != null) { 
            _scrollRect.content.GetComponentsInChildren(_selectables);
        }
        ScrollToSelected(true);
    }

    void Update() {
        InputScroll();
        if (!_isHoovered) { 
            _scrollRect.normalizedPosition = Vector2.Lerp(_scrollRect.normalizedPosition, _newScrollBarPos, ScrollSpeed * Time.unscaledDeltaTime);
        }
        else {
            _newScrollBarPos = _scrollRect.normalizedPosition;
        }
    }

    void InputScroll() { 
        if(_selectables.Count > 0) {
            ScrollToSelected(false);
        }
    }

    void ScrollToSelected(bool isQuickScroll) {
        int selectedID = -1;
        Selectable selectedItem = EventSystem.current.currentSelectedGameObject ? EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>() : null;
        if (selectedItem != null) { 
            selectedID = _selectables.IndexOf(selectedItem);
        }
        if(selectedID > -1) {
            if(isQuickScroll) { 
                _scrollRect.normalizedPosition = new Vector2(0,1-(selectedID/((float)_selectables.Count-1)));
                _newScrollBarPos = _scrollRect.normalizedPosition;
            }
            else {
                _newScrollBarPos = new Vector2(0, 1 - (selectedID / ((float)_selectables.Count - 1)));
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        _isHoovered = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        _isHoovered = false;
        ScrollToSelected(false);
    }
}
