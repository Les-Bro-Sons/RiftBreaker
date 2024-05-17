using TMPro;
using UnityEngine;

public class RB_PlayerAnim : MonoBehaviour
{
    //Components
    Animator _playerAnimator;
    RB_PlayerMovement _playerMovement;
    Transform _transform;

    private void Awake()
    {
        _playerAnimator = GetComponentInChildren<Animator>();
        _playerMovement = GetComponent<RB_PlayerMovement>();
        _transform = transform;
    }

    private void UpdateAnimation()
    {
        _playerAnimator.SetFloat("Speed", _playerMovement.GetVelocity().magnitude);
        _playerAnimator.SetFloat("yRot", _transform.rotation.eulerAngles.y);
    }

    private void Update()
    {
        UpdateAnimation();
    }
}
