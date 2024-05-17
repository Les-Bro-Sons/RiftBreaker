using TMPro;
using UnityEngine;

public class RB_PlayerMovement : MonoBehaviour
{
    //Player movement properties
    [Header("Player movement properties")]
    [HideInInspector] public Vector3 LastDirection;
    [SerializeField] private float _movementAcceleration;
    [SerializeField] private float _movementMaxSpeed;
    [SerializeField] private float _movementFrictionForce;
    private Vector3 _currentVelocity;
    private bool _isMoving = false;
    private bool _canMove = true;

    //Dash properties
    [Header("Dash properties")]
    [SerializeField] private float _dashCooldown;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashDistance;
    private Vector3 _dashEndPos;
    private Vector3 _dashDirection;
    private bool _canDash = true;
    private bool _isDashing = false;
    private float _lastUsedDashTime = 0;

    //Components
    [Header("Components")]
    private Rigidbody _rb;
    private Transform _transform;

    //Debug components
    [Header("Debug Components")]
    [SerializeField] private TextMeshProUGUI _debugSpeedText;

    //Debug properties
    private Vector3 _previousPosition;
    private Vector3 _currentPosition;
    private void Awake()
    {
        _rb = GetComponentInChildren<Rigidbody>();
        _transform = transform;
    }
    private void Start()
    {
        RB_InputManager.Instance.EventMovePerformed.AddListener(StartMove);
        RB_InputManager.Instance.EventMoveCanceled.AddListener(StopMove);
        RB_InputManager.Instance.EventDashStarted.AddListener(StartDash);
        //Dash cooldown
        Invoke("DebugSpeed", 0);
    }

    //Starting movement
    public void StartMove()
    {
        _isMoving = true;
    }

    //Stopping movement
    public void StopMove()
    {
        _isMoving = false;
    }

    //Moving the player
    public void Move(Vector3 direction)
    {
        //Getting the direction of the movement
        Vector3 directionToMove = new Vector3(direction.x, 0, direction.y);
        if (_isMoving)
        {
            if (_canMove)
            {
                if(_currentVelocity.magnitude < _movementMaxSpeed)
                {
                    //Adding velocity to player
                    _rb.AddForce(directionToMove * _movementMaxSpeed * Time.fixedDeltaTime * _movementAcceleration);
                }
            }
            //Setting the direction to the player
            _transform.forward = directionToMove;
        }
        Vector3 frictionForce = (-_rb.velocity) * Time.fixedDeltaTime * _movementFrictionForce;
        _rb.AddForce(new Vector3(frictionForce.x, 0, frictionForce.z)); //Friction force (stop the movement)

        //Clamping to max speed in the x and z axes
        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, _movementMaxSpeed);
        _rb.velocity = new Vector3(horizontalVelocity.x, _rb.velocity.y, horizontalVelocity.z);
    }

    public void SetSpeed()
    {
        //Calculating the real time speed
        _currentPosition = transform.position;
        _currentVelocity = (_currentPosition - _previousPosition) / Time.deltaTime;
        _previousPosition = _currentPosition;
    }

    
    public void StartDash()
    {
        //Starting dash
        if (CanDash()){
            _dashEndPos = _transform.position + _transform.forward * _dashDistance;
            _dashDirection = _transform.forward;
            _lastUsedDashTime = Time.time;
            _isDashing = true;
            _canMove = false;
        }
    }

    
    public void StopDash()
    {
        //Stopping dash
        _isDashing = false;
        _canMove = true;
    }

    public void Dash()
    {
        if(_isDashing)
        {
            //Dashing
            _rb.velocity = _dashSpeed * _dashDirection;
            if (Vector3.Distance(_rb.position, _dashEndPos) < 0.5f)
            {
                StopDash();
            }
        }
    }

    public bool CanDash()
    {
        //Cooldown dash
        return _canDash && Time.time > (_lastUsedDashTime + _dashCooldown);
    }

    private void DebugSpeed()
    {
        //Printing debug speed to screen
        _debugSpeedText.text = ((int)_currentVelocity.magnitude).ToString();
        Invoke("DebugSpeed", 0.1f);
    }

    private void FixedUpdate()
    {
        //Calling the speed calcul in real time
        SetSpeed();

        //Call the movement function
        Move(RB_InputManager.Instance.MoveValue);

        //Call the dash function
        Dash();

        
    }


}
