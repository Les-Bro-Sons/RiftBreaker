using System;
using UnityEngine.UI;

////TODO: have updateBindingUIEvent receive a control path string, too (in addition to the device layout name)

namespace UnityEngine.InputSystem.Samples.RebindUI
{
    /// <summary>
    /// This is an example for how to override the default display behavior of bindings. The component
    /// hooks into <see cref="RebindActionUI.updateBindingUIEvent"/> which is triggered when UI display
    /// of a binding should be refreshed. It then checks whether we have an icon for the current binding
    /// and if so, replaces the default text display with an icon.
    /// </summary>
    public class InputIcons : MonoBehaviour
    {
        public GamepadIcons xbox;
        public GamepadIcons ps4;
        public MouseIcons mouse;
        public KeyboardIcons keyboard;

        protected void OnEnable()
        {
            // Hook into all updateBindingUIEvents on all RebindActionUI components in our hierarchy.
            var rebindUIComponents = transform.GetComponentsInChildren<RebindActionUI>();
            foreach (var component in rebindUIComponents)
            {
                component.updateBindingUIEvent.AddListener(OnUpdateBindingDisplay);
                component.UpdateBindingDisplay();
            }
        }

        protected void OnUpdateBindingDisplay(RebindActionUI component, string bindingDisplayString, string deviceLayoutName, string controlPath)
        {
            if (string.IsNullOrEmpty(deviceLayoutName) || string.IsNullOrEmpty(controlPath))
                return;

            var icon = default(Sprite);
            if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "DualShockGamepad"))
                icon = ps4.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Gamepad"))
                icon = xbox.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Mouse"))
                icon = mouse.GetSprite(controlPath);
            else if (InputSystem.IsFirstLayoutBasedOnSecond(deviceLayoutName, "Keyboard")) {
                icon = keyboard.GetSprite(controlPath);
            }


            var textComponent = component.bindingText;

            // Grab Image component.
            var imageGO = textComponent.transform.parent.Find("ActionBindingIcon");
            var imageComponent = imageGO.GetComponent<Image>();

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

        [Serializable]
        public struct GamepadIcons {
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

            public Sprite GetSprite(string controlPath) {
                // From the input system, we get the path of the control on device. So we can just
                // map from that to the sprites we have for gamepads.
                switch (controlPath) {
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
        public struct MouseIcons {
            public Sprite LMB;
            public Sprite RMB;
            public Sprite MMB;

            public Sprite GetSprite(string controlPath) {

                switch (controlPath) {
                    case "rightButton":return RMB;
                    case "leftButton":return LMB;
                    case "middleButton":return MMB;
                }
            
                return null;
            }
        }

        [Serializable]
        public struct KeyboardIcons
        {
            //letter
            public Sprite A;
            public Sprite B;
            public Sprite C;
            public Sprite D;
            public Sprite E;
            public Sprite F;
            public Sprite G;
            public Sprite H;
            public Sprite I;
            public Sprite J;
            public Sprite K;
            public Sprite L;
            public Sprite M;
            public Sprite N;
            public Sprite O;
            public Sprite P;
            public Sprite Q;
            public Sprite R;
            public Sprite S;
            public Sprite T;
            public Sprite U;
            public Sprite V;
            public Sprite W;
            public Sprite X;
            public Sprite Y;
            public Sprite Z;

            //Numbers
            public Sprite Zero;
            public Sprite One;
            public Sprite Two;
            public Sprite Three;
            public Sprite Four;
            public Sprite Five;
            public Sprite Six;
            public Sprite Seven;
            public Sprite Eight;
            public Sprite Nine;

            //Other
            public Sprite Space;

            public Sprite GetSprite(string controlPath)
            {
                switch (controlPath)
                {
                    case "q": return A;
                    case "b": return B;
                    case "c": return C;
                    case "d": return D;
                    case "e": return E;
                    case "f": return F;
                    case "g": return G;
                    case "h": return H;
                    case "i": return I;
                    case "j": return J;
                    case "k": return K;
                    case "l": return L;
                    case "semicolon": return M;
                    case "n": return N;
                    case "o": return O;
                    case "p": return P;
                    case "a": return Q;
                    case "r": return R;
                    case "s": return S;
                    case "t": return T;
                    case "u": return U;
                    case "v": return V;
                    case "w": return Z;
                    case "x": return X;
                    case "y": return Y;
                    case "z": return W;
                    case "0": return Zero;
                    case "1": return One;
                    case "2": return Two;
                    case "3": return Three;
                    case "4": return Four;
                    case "5": return Five;
                    case "6": return Six;
                    case "7": return Seven;
                    case "8": return Eight;
                    case "9": return Nine;
                    case "numpad0": return Zero;
                    case "space": return Space;
                }

                return null;
            }
        }
    }
}
