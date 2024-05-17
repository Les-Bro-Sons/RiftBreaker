using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_HUDHourglass : MonoBehaviour {

    Animator _animator;
    void Awake() {
        _animator = GetComponent<Animator>();
    }

    public void ResetAnim() { 
    
    }

    public void StartAnim() {
        _animator.SetTrigger("IsReversed");
    }
}
