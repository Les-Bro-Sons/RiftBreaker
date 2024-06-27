namespace UnityEngine.InputSystem.Samples.RebindUI
{
    /// <summary>
    /// This is an example for how to override the default display behavior of bindings. The component
    /// hooks into <see cref="RB_InputToolTipCombo.updateBindingUIEvent"/> which is triggered when UI display
    /// of a binding should be refreshed. It then checks whether we have an icon for the current binding
    /// and if so, replaces the default text display with an icon.
    /// </summary>
    public class InputIcons_ToolTipCombo : MonoBehaviour{
        // Start is called before the first frame update
        protected void Start(){
            // Hook into all updateBindingUIEvents on all RebindActionUI components in our hierarchy.
            RB_InputToolTipCombo ComboCoponent = transform.GetComponent<RB_InputToolTipCombo>();
            ComboCoponent.updateBindingUIEvent.AddListener(OnUpdateBindingDisplay);
            ComboCoponent.updateModifierUIEvent.AddListener(OnUpdateModifierDisplay);
            ComboCoponent.UpdateBindingDisplay();

        }

        protected void OnUpdateBindingDisplay(RB_InputToolTipCombo component, string bindingDisplayString, string deviceLayoutName, string controlPath) {
            if (string.IsNullOrEmpty(deviceLayoutName) || string.IsNullOrEmpty(controlPath))
                return;

            var icon = default(Sprite);
            if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "DualShockGamepad"))
                icon = RB_IconsSprite.Instance.Ps4.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Gamepad"))
                icon = RB_IconsSprite.Instance.Xbox.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Mouse"))
                icon = RB_IconsSprite.Instance.Mouse.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Keyboard")) {
                icon = RB_IconsSprite.Instance.Keyboard.GetSprite(controlPath);
            }


            GameObject textParent = component.TextParent;
            GameObject imageParent = component.ImageParent;


            // Grab Image component.
            var imageComponent = component.bindingImage;

            if (icon != null){
                textParent.SetActive(false);
                imageComponent.sprite = icon;
                imageComponent.preserveAspect = true;
                imageParent.SetActive(true);
            }
            else
            {
                textParent.SetActive(true);
                imageParent.SetActive(false);
            }
        }
    protected void OnUpdateModifierDisplay(RB_InputToolTipCombo component, string bindingDisplayString, string deviceLayoutName, string controlPath) {
            if (string.IsNullOrEmpty(deviceLayoutName) || string.IsNullOrEmpty(controlPath))
                return;

            var icon = default(Sprite);
            if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "DualShockGamepad"))
                icon = RB_IconsSprite.Instance.Ps4.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Gamepad"))
                icon = RB_IconsSprite.Instance.Xbox.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Mouse"))
                icon = RB_IconsSprite.Instance.Mouse.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Keyboard")) {
                icon = RB_IconsSprite.Instance.Keyboard.GetSprite(controlPath);
            }

            GameObject textParent = component.TextParent;
            GameObject imageParent = component.ImageParent;

            // Grab Image component.
            var imageComponent = component.modifierImage;

            if (icon != null){
                textParent.SetActive(false);
                imageComponent.sprite = icon;
                imageComponent.preserveAspect = true;
                imageParent.SetActive(true);
            }
            else
            {
                textParent.SetActive(true);
                imageParent.SetActive(false);
            }
        }
    }
}
