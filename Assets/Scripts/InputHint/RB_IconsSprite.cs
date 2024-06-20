using System;
using UnityEngine;

public class RB_IconsSprite : MonoBehaviour{
    [SerializeField] public GamepadIcons Xbox;
    public GamepadIcons Ps4;
    public MouseIcons Mouse;
    public KeyboardIcons Keyboard;

    public static RB_IconsSprite Instance;

    private void Awake(){
        if(Instance == null) { Debug.Log("NIGGA"); Instance = this;}
    }

    [Serializable]
    public struct GamepadIcons {
        //buttons
        public Sprite ButtonSouth;
        public Sprite ButtonNorth;
        public Sprite ButtonEast;
        public Sprite ButtonWest;
        public Sprite Select;
        //triggers/shoulder
        public Sprite LeftTrigger;
        public Sprite RightTrigger;
        public Sprite LeftShoulder;
        public Sprite RightShoulder;
        //Dpad
        public Sprite Dpad;
        public Sprite DpadUp;
        public Sprite DpadDown;
        public Sprite DpadLeft;
        public Sprite DpadRight;
        //LeftStick
        public Sprite LeftStick;
        public Sprite LeftStickPress;
        public Sprite LeftStickUp;
        public Sprite LeftStickDown;
        public Sprite LeftStickLeft;
        public Sprite LeftStickRight;
        //RightStick
        public Sprite RightStick;
        public Sprite RightStickPress;
        public Sprite RightStickUp;
        public Sprite RightStickDown;
        public Sprite RightStickLeft;
        public Sprite RightStickRight;

        public Sprite GetSprite(string controlPath) {
            // From the input system, we get the path of the control on device. So we can just
            // map from that to the sprites we have for gamepads.
            switch (controlPath) {
                case "buttonSouth": return ButtonSouth;
                case "buttonNorth": return ButtonNorth;
                case "buttonEast": return ButtonEast;
                case "buttonWest": return ButtonWest;
                case "select": return Select;
                case "leftTrigger": return LeftTrigger;
                case "rightTrigger": return RightTrigger;
                case "leftShoulder": return LeftShoulder;
                case "rightShoulder": return RightShoulder;
                case "dpad": return Dpad;
                case "dpad/up": return DpadUp;
                case "dpad/down": return DpadDown;
                case "dpad/left": return DpadLeft;
                case "dpad/right": return DpadRight;
                case "leftStick": return LeftStick;
                case "leftStickPress": return LeftStickPress;
                case "leftStick/up": return LeftStickUp;
                case "leftStick/down": return LeftStickDown;
                case "leftStick/left": return LeftStickLeft;
                case "leftStick/right": return LeftStickRight;
                case "rightStick": return RightStick;
                case "rightStickPress": return RightStickPress;
                case "rightStick/up": return RightStickUp;
                case "rightStick/down": return RightStickDown;
                case "rightStick/left": return RightStickLeft;
                case "rightStick/right": return RightStickRight;
            }
            return null;
        }
    }

    [Serializable]
    public struct MouseIcons {
        public Sprite LMB;
        public Sprite RMB;
        public Sprite MMB;

        public Sprite GetSprite(string controlPath){

            switch (controlPath) {
                case "rightButton": return RMB;
                case "leftButton": return LMB;
                case "middleButton": return MMB;
            }

            return null;
        }
    }

    [Serializable]
    public struct KeyboardIcons{
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

        public Sprite GetSprite(string controlPath){
            Debug.Log("Gyat");
            switch (controlPath) {
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
                case "&": Debug.Log("nigga2"); return One;
                case "1": Debug.Log("Nigga"); return One;
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
