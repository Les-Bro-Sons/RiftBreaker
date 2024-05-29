using UnityEngine;

public class RB_FollowPlayer : MonoBehaviour
{
    //Components
    Transform _transform;
    private Rigidbody _rb;

    //Player
    private Transform _playerTransform;
    

    private void Awake()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _playerTransform = RB_PlayerMovement.Instance.transform;

    }


    void Update()
    {
        _rb.MovePosition(_playerTransform.position);
    }
}
