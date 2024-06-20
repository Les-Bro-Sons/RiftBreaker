using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RB_TooltipManager : MonoBehaviour {

    public List<GameObject> KeyboardTooltips = new List<GameObject>();
    public List<GameObject> GamepadTooltips = new List<GameObject>();


    private void Start(){
        foreach (GameObject tooltip in KeyboardTooltips) {
            tooltip.SetActive(true);
        }
        foreach (GameObject tooltip in GamepadTooltips){
            tooltip.SetActive(false);
        }
    }

    private void Update() {
        Debug.Log(RB_InputManager.Instance.IsKeyBoard);
        foreach (GameObject tooltip in KeyboardTooltips){
            tooltip.SetActive(RB_InputManager.Instance.IsKeyBoard);
        }
        foreach (GameObject tooltip in GamepadTooltips){
            tooltip.SetActive(!RB_InputManager.Instance.IsKeyBoard);
        }
    }
}
