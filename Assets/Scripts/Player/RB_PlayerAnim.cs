using UnityEngine;

public class RB_PlayerAnim : MonoBehaviour
{
    //Components
    [SerializeField] Animator _playerAnimator;
    RB_PlayerMovement _playerMovement;

    private void Awake()
    {
        _playerMovement = GetComponent<RB_PlayerMovement>();
    }

    private void UpdateAnimation()
    {
        // Reset all directions to false
        _playerAnimator.SetBool("Back", false);
        _playerAnimator.SetBool("Face", false);
        _playerAnimator.SetBool("Right", false);
        _playerAnimator.SetBool("Left", false);

        // Set the actual direction to true
        switch (_playerMovement.ActualDirection)
        {
            case RB_PlayerMovement.Direction.Back:
                _playerAnimator.SetBool("Back", true);
                break;
            case RB_PlayerMovement.Direction.Face:
                _playerAnimator.SetBool("Face", true);
                break;
            case RB_PlayerMovement.Direction.Left:
                _playerAnimator.SetBool("Left", true);
                break;
            case RB_PlayerMovement.Direction.Right:
                _playerAnimator.SetBool("Right", true);
                break;
            default:
                break;
        }

        //If the speed is near 0, the idle animation is started
        if(_playerMovement.GetVelocity().magnitude < .1f)
            _playerAnimator.SetBool("Idle", true);
        else
            _playerAnimator.SetBool("Idle", false);
    }

    private void Update()
    {
        //Animate constantly
        UpdateAnimation();
    }
}
