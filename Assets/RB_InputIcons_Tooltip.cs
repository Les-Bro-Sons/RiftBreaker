////TODO: have updateBindingUIEvent receive a control path string, too (in addition to the device layout name)

namespace UnityEngine.InputSystem.Samples.RebindUI {
    /// <summary>
    /// This is an example for how to override the default display behavior of bindings. The component
    /// hooks into <see cref="RB_InputToolTip.updateBindingUIEvent"/> which is triggered when UI display
    /// of a binding should be refreshed. It then checks whether we have an icon for the current binding
    /// and if so, replaces the default text display with an icon.
    /// </summary>
    public class InputIcons_ToolTip : MonoBehaviour {

        protected void Start() {
            // Hook into all updateBindingUIEvents on all RebindActionUI components in our hierarchy.
            RB_InputToolTip[] rebindUIComponents = transform.GetComponentsInChildren<RB_InputToolTip>(true);
            Debug.Log(rebindUIComponents.Length);
            foreach (RB_InputToolTip component in rebindUIComponents){
                component.updateBindingUIEvent.AddListener(OnUpdateBindingDisplay);
                component.UpdateBindingDisplay();
            }
        }

        protected void OnUpdateBindingDisplay(RB_InputToolTip component, string bindingDisplayString, string deviceLayoutName, string controlPath)  {
            
            if (string.IsNullOrEmpty(deviceLayoutName) || string.IsNullOrEmpty(controlPath))
                return;

            //Debug.Log(deviceLayoutName);
            
            var icon = default(Sprite);
            if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "DualShockGamepad"))
                icon = RB_IconsSprite.Instance.Ps4.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Gamepad"))
                icon = RB_IconsSprite.Instance.Xbox.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Mouse"))
                icon = RB_IconsSprite.Instance.Mouse.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Keyboard")) {
                Debug.Log(RB_IconsSprite.Instance== null);
                
                icon = RB_IconsSprite.Instance.Keyboard.GetSprite(controlPath);
            }
                

                //Debug.Log(icon == null);
                var textComponent = component.bindingText;


            // Grab Image component.
            var imageComponent = component.bindingImage;

            if (icon != null) {
                textComponent.gameObject.SetActive(false);
                imageComponent.sprite = icon;
                imageComponent.preserveAspect = true;
                imageComponent.gameObject.SetActive(true);
            }
            else {
                textComponent.gameObject.SetActive(true);
                imageComponent.gameObject.SetActive(false);
            }
        }
    }
}
