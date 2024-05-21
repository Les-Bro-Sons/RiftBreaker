using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_HUDHourglass : MonoBehaviour {

    Animator _animator;
    void Awake() {
        _animator = GetComponent<Animator>();
    }

    //Pour lancer le renversement du sablier
    public void StartAnim() {
        _animator.SetTrigger("IsReversed");
    }
}
