using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_BossRoom : MonoBehaviour{

    RB_Room _room;

    private void Start()
    {
        _room = GetComponent<RB_Room>();
    }

    void Update() {
        if (_room.IsPlayerInRoom)
        {
            RB_InputManager.Instance.AttackEnabled = false;
            RB_InputManager.Instance.SpecialAttackEnabled = false;
        }
        else{
            RB_InputManager.Instance.AttackEnabled = true;
            RB_InputManager.Instance.SpecialAttackEnabled = true;
        }
    }
}
