using System;
using UnityEngine;

namespace UnityEngine.InputSystem.Samples.RebindUI
{
    public class RB_InputIcons_ShowBindText : MonoBehaviour
    {
        public GamepadIcons Xbox;
        public GamepadIcons Ps4;
        public MouseIcons Mouse;

        protected void OnEnable()
        {
            // Hook into all updateBindingUIEvents on all RebindActionUI components in our hierarchy.
            var rebindUIComponents = transform.GetComponentsInChildren<RB_ShowBindText>();
            foreach (var component in rebindUIComponents)
            {
                component.updateBindingUIEvent.AddListener(OnUpdateBindingDisplay);
                component.UpdateBindingDisplay();
            }
        }

        protected void OnUpdateBindingDisplay(RB_ShowBindText component, string bindingDisplayString, string deviceLayoutName, string controlPath)
        {

            if (string.IsNullOrEmpty(deviceLayoutName) || string.IsNullOrEmpty(controlPath))
                return;

            var icon = default(Sprite);
            if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "DualShockGamepad"))
                icon = Ps4.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Gamepad"))
                icon = Xbox.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Mouse"))
                icon = Mouse.GetSprite(controlPath);

            var textComponent = component.bindingText;


            // Grab Image component.
            var imageComponent = component.bindingImage;

            if (icon != null)
            {
                textComponent.gameObject.SetActive(false);
                imageComponent.sprite = icon;
                imageComponent.preserveAspect = true;
                imageComponent.gameObject.SetActive(true);
            }
            else
            {
                textComponent.gameObject.SetActive(true);
                imageComponent.gameObject.SetActive(false);
            }
        }

        [Serializable]
        public struct GamepadIcons
        {
            //buttons
            public Sprite buttonSouth;
            public Sprite buttonNorth;
            public Sprite buttonEast;
            public Sprite buttonWest;
            public Sprite select;
            //triggers/shoulder
            public Sprite leftTrigger;
            public Sprite rightTrigger;
            public Sprite leftShoulder;
            public Sprite rightShoulder;
            //dpad
            public Sprite dpad;
            public Sprite dpadUp;
            public Sprite dpadDown;
            public Sprite dpadLeft;
            public Sprite dpadRight;
            //leftStick
            public Sprite leftStick;
            public Sprite leftStickPress;
            public Sprite leftStickUp;
            public Sprite leftStickDown;
            public Sprite leftStickLeft;
            public Sprite leftStickRight;
            //rightStick
            public Sprite rightStick;
            public Sprite rightStickPress;
            public Sprite rightStickUp;
            public Sprite rightStickDown;
            public Sprite rightStickLeft;
            public Sprite rightStickRight;

            public Sprite GetSprite(string controlPath)
            {
                // From the input system, we get the path of the control on device. So we can just
                // map from that to the sprites we have for gamepads.
                switch (controlPath)
                {
                    case "buttonSouth": return buttonSouth;
                    case "buttonNorth": return buttonNorth;
                    case "buttonEast": return buttonEast;
                    case "buttonWest": return buttonWest;
                    case "select": return select;
                    case "leftTrigger": return leftTrigger;
                    case "rightTrigger": return rightTrigger;
                    case "leftShoulder": return leftShoulder;
                    case "rightShoulder": return rightShoulder;
                    case "dpad": return dpad;
                    case "dpad/up": return dpadUp;
                    case "dpad/down": return dpadDown;
                    case "dpad/left": return dpadLeft;
                    case "dpad/right": return dpadRight;
                    case "leftStick": return leftStick;
                    case "leftStickPress": return leftStickPress;
                    case "leftStick/up": return leftStickUp;
                    case "leftStick/down": return leftStickDown;
                    case "leftStick/left": return leftStickLeft;
                    case "leftStick/right": return leftStickRight;
                    case "rightStick": return rightStick;
                    case "rightStickPress": return rightStickPress;
                    case "rightStick/up": return rightStickUp;
                    case "rightStick/down": return rightStickDown;
                    case "rightStick/left": return rightStickLeft;
                    case "rightStick/right": return rightStickRight;
                }
                return null;
            }
        }

        [Serializable]
        public struct MouseIcons
        {
            public Sprite LMB;
            public Sprite RMB;
            public Sprite MMB;

            public Sprite GetSprite(string controlPath)
            {

                switch (controlPath)
                {
                    case "rightButton": return RMB;
                    case "leftButton": return LMB;
                    case "middleButton": return MMB;
                }

                return null;
            }
        }
    }
}
