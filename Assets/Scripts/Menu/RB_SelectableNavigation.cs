using UnityEngine;
using UnityEngine.UI;

// Ensure that this component requires a Selectable component to be attached
[RequireComponent(typeof(Selectable))]
public class RB_SelectableNavigation : MonoBehaviour
{
    // Navigation structs for different menu states
    public UINavigation AudioNavigation;
    public UINavigation VideoNavigation;
    public UINavigation ControlKeyboardNavigation;
    public UINavigation ControlGamepadNavigation;

    private Selectable _selectable;

    private void Start()
    {
        // Get the Selectable component attached to this GameObject
        _selectable = GetComponent<Selectable>();

        // Initialize navigation structs with the current Selectable instance
        AudioNavigation.Initialize(_selectable);
        VideoNavigation.Initialize(_selectable);
        ControlKeyboardNavigation.Initialize(_selectable);
        ControlGamepadNavigation.Initialize(_selectable);
    }

    void Update()
    {
        // Check the current menu state from the menu manager singleton
        switch (RB_MenuManager.Instance.MenuState)
        {
            case RB_MenuManager.MENUSTATE.Audio:
                // Apply navigation for the Audio menu state
                AudioNavigation.ApplyNavigation();
                break;
            case RB_MenuManager.MENUSTATE.Video:
                // Apply navigation for the Video menu state
                VideoNavigation.ApplyNavigation();
                break;
            case RB_MenuManager.MENUSTATE.Control:
                // Check the current control binder type from the control manager singleton
                if (RB_MenuControlManager.Instance.CurrentBinder == RB_MenuControlManager.BINDERS.keyboard)
                {
                    // Apply navigation for keyboard controls
                    ControlKeyboardNavigation.ApplyNavigation();
                }
                else if (RB_MenuControlManager.Instance.CurrentBinder == RB_MenuControlManager.BINDERS.controller)
                {
                    // Apply navigation for gamepad/controller controls
                    ControlGamepadNavigation.ApplyNavigation();
                }
                break;
        }
    }

    // Serializable struct for defining navigation for a Selectable
    [System.Serializable]
    public struct UINavigation
    {
        public Selectable Up;    // Selectable to navigate to when pressing Up
        public Selectable Down;  // Selectable to navigate to when pressing Down
        public Selectable Left;  // Selectable to navigate to when pressing Left
        public Selectable Right; // Selectable to navigate to when pressing Right

        private Selectable _selectable;

        // Initialize the navigation struct with a Selectable instance
        public void Initialize(Selectable selectable)
        {
            _selectable = selectable;
        }

        // Apply the defined navigation to the Selectable
        public void ApplyNavigation()
        {
            Navigation navigation = _selectable.navigation;
            navigation.selectOnUp = Up;
            navigation.selectOnDown = Down;
            navigation.selectOnLeft = Left;
            navigation.selectOnRight = Right;
            _selectable.navigation = navigation;
        }
    }
}
