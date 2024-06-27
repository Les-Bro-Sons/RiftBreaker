using UnityEngine;

public class RB_HUDHourglass : MonoBehaviour {

    Animator _animator;
    void Awake() {
        _animator = GetComponent<Animator>();
    }

    //Reverse the Hourglass
    public void StartAnim() {
        _animator.SetTrigger("IsReversed");
    }
}
