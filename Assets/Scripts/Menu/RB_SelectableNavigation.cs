using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class RB_SelectableNavigation : MonoBehaviour{
    public UINavigation AudioNavigation;
    public UINavigation VideoNavigation;
    public UINavigation ControlKeyboardNavigation;
    public UINavigation ControlGamepadNavigation;

    private Selectable _selectable;

    private void Start() {
        _selectable = GetComponent<Selectable>();

        // Initialize the navigation structs with the current Selectable instance
        AudioNavigation.Initialize(_selectable);
        VideoNavigation.Initialize(_selectable);
        ControlKeyboardNavigation.Initialize(_selectable);
        ControlGamepadNavigation.Initialize(_selectable);
    }

    void Update()
    {
        switch (RB_MenuManager.Instance.MenuState)
        {
            case RB_MenuManager.MENUSTATE.Audio:
                AudioNavigation.ApplyNavigation();
                break;
            case RB_MenuManager.MENUSTATE.Video:
                VideoNavigation.ApplyNavigation();
                break;
            case RB_MenuManager.MENUSTATE.Control:
                if (RB_MenuControlManager.Instance.CurrentBinder == RB_MenuControlManager.BINDERS.keyboard) {
                    ControlKeyboardNavigation.ApplyNavigation();
                }
                else if (RB_MenuControlManager.Instance.CurrentBinder == RB_MenuControlManager.BINDERS.controller) {
                    ControlGamepadNavigation.ApplyNavigation();
                }
                break;
        }
    }

    [System.Serializable]
    public struct UINavigation {
        public Selectable Up;
        public Selectable Down;
        public Selectable Left;
        public Selectable Right;

        private Selectable _selectable;

        public void Initialize(Selectable selectable) {
            _selectable = selectable;
        }

        public void ApplyNavigation() {
            Navigation navigation = _selectable.navigation;
            navigation.selectOnUp = Up;
            navigation.selectOnDown = Down;
            navigation.selectOnLeft = Left;
            navigation.selectOnRight = Right;
            _selectable.navigation = navigation;
        }
    }
}
