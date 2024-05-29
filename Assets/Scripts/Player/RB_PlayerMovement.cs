using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RB_PlayerMovement : MonoBehaviour
{
    //Enums
    public enum Direction { Back, Face, Left, Right}

    //Player movement properties
    [Header("Player movement properties")]
    [HideInInspector] public Vector3 LastDirection;
    [SerializeField] private float _movementAcceleration;
    [SerializeField] private float _movementMaxSpeed;
    [SerializeField] private float _movementFrictionForce;
    public Vector3 DirectionToMove;
    public Direction ActualDirection;
    private Vector3 _currentVelocity;
    private bool _isMoving = false;
    private bool _canMove = true;

    //Dash properties
    [Header("Dash properties")]
    [SerializeField] private float _dashCooldown; public float DashCooldown { get { return _dashCooldown; } }
    [SerializeField] private float _dashSpeed;
    [SerializeField] private float _dashDistance;
    [SerializeField] private float _fadeOutInterval;
    [SerializeField] private float _fadeForce;
    [SerializeField] private float _zFadeOffset;
    [SerializeField] private GameObject _spritePrefab;
    [SerializeField] private float _totalDashTime;
    private Vector3 _dashDirection;
    private Vector3 _firstDashPosition;
    private bool _canDash = true;
    private bool _isDashing = false;
    private float _lastUsedDashTime = 0;
    [HideInInspector] public UnityEvent EventDash;

    //Components
    [Header("Components")]
    private Rigidbody _rb;
    private Transform _transform;
    private RB_PlayerAction _playerAction;

    //Debug components
    [Header("Debug Components")]
    [SerializeField] private TextMeshProUGUI _debugSpeedText;

    //Debug properties
    private Vector3 _previousPosition;
    private Vector3 _currentPosition;

    public static RB_PlayerMovement Instance;
    private void Awake()
    {   
        Instance = this;
        _rb = GetComponentInChildren<Rigidbody>();
        _transform = transform;
        _playerAction = GetComponent<RB_PlayerAction>();
        ResetDirection();
    }
    private void Start()
    {
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

    public void GetDirection(Vector3 direction)
    {
        DirectionToMove = new Vector3(direction.x, 0, direction.y);
        if (!_playerAction.IsAttacking)
        {
            //setting the right direction in the state
            if (Mathf.Abs(DirectionToMove.x) > Mathf.Abs(DirectionToMove.y))
            {
                if (DirectionToMove.x > 0)
                    ActualDirection = Direction.Right;
                else
                    ActualDirection = Direction.Left;
            }
            else
            {
                if (DirectionToMove.z > 0)
                    ActualDirection = Direction.Back;
                else
                    ActualDirection = Direction.Face;
            }
        }
    }

    public void SetDirection()
    {
        if (!_playerAction.IsDoingAnyAttack())
        {
            //Setting the direction to the player
            _rb.MoveRotation(Quaternion.LookRotation(DirectionToMove));
        }
    }

    public Vector3 GetDirectionToMove()
    {
        return DirectionToMove;
    }

    public void ResetDirection()
    {
        ActualDirection = Direction.Face;
        _rb.MoveRotation(Quaternion.LookRotation(Vector3.back));
        DirectionToMove = Vector3.back;
    }

    private void ClampingSpeed()
    {
        //Clamping to max speed in the x and z axes
        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, _movementMaxSpeed);
        _rb.velocity = new Vector3(horizontalVelocity.x, _rb.velocity.y, horizontalVelocity.z);
    }

    private void FrictionForce()
    {
        Vector3 frictionForce = (-_rb.velocity) * Time.fixedDeltaTime * _movementFrictionForce;
        _rb.AddForce(new Vector3(frictionForce.x, 0, frictionForce.z)); //Friction force (stop the movement)
    }

    //Moving the player
    public void Move()
    {
        if(_currentVelocity.magnitude < _movementMaxSpeed)
        {
            //Adding velocity to player
            _rb.AddForce(DirectionToMove * _movementMaxSpeed * Time.fixedDeltaTime * _movementAcceleration);
        }
    }

    public bool CanMove()
    {
        //if is moving, not dashing and not attacking
        return _canMove && _isMoving && !_isDashing && !_playerAction.IsDoingAnyNotNormalAttack() || (_playerAction.IsSpecialAttacking && _playerAction.CurrentItem.CanMoveDuringSpecialAttack);
    }

    private void SetSpeed()
    {
        //Calculating the real time speed
        _currentPosition = transform.position;
        _currentVelocity = (_currentPosition - _previousPosition) / Time.deltaTime;
        _previousPosition = _currentPosition;
    }

    public Vector3 GetVelocity()
    {
        //Get the current velocity that we can convert to speed
        return _currentVelocity;
    }

    public void DashAnim()
    {
        //Create animation for dashing
        if (_isDashing)
        {
            //Spawn a "white shadow" behind the player
            GameObject spawnedSprite =  Instantiate(_spritePrefab, new Vector3(_transform.position.x, 1, _previousPosition.z + _zFadeOffset), Quaternion.identity);
            spawnedSprite.GetComponent<RB_SpriteFadeOut>().FadeForce = _fadeForce;
            Invoke("DashAnim", _fadeOutInterval);
        }
    }
    
    public void StartDash()
    {
        //Starting dash
        _firstDashPosition = _transform.position;
        _dashDirection = (RB_InputManager.Instance.MoveValue.magnitude >= .1f )  ? new Vector3(RB_InputManager.Instance.MoveValue.x, 0, RB_InputManager.Instance.MoveValue.y) : -_transform.forward;
        _lastUsedDashTime = Time.time;
        _isDashing = true;
        //Starting dash animation
        DashAnim();
    }

    
    public void StopDash()
    {
        //Stopping dash
        _isDashing = false;
    }

    public void Dash()
    {
        if(_isDashing)
        {
            //Dashing
            float distanceThisFrame = Vector3.Distance(_firstDashPosition, _transform.position);
            if (distanceThisFrame >= _dashDistance || (Time.time >= _lastUsedDashTime + _totalDashTime && _currentVelocity.magnitude <= 1))
            {
                StopDash();
                EventDash.Invoke();
            }
            _rb.velocity = _dashSpeed * _dashDirection;
        }
    }

    public bool CanDash()
    {
        //Cooldown dash and not attacking
        return _canDash && Time.time > (_lastUsedDashTime + _dashCooldown) && !_playerAction.IsDoingAnyNotNormalAttack();
    }

    private void DebugSpeed()
    {
        //Printing debug speed to screen
        if(_debugSpeedText != null)
        {
            _debugSpeedText.text = ((int)_currentVelocity.magnitude).ToString();
            Invoke("DebugSpeed", 0.1f);
        }
    }

    private void Update()
    {
        //Set the direction
        SetDirection();
    }

    private void FixedUpdate()
    {
        //Clamping the speed to the max speed
        ClampingSpeed();

        //Adding friction force
        FrictionForce();

        //Calling the speed calcul in real time
        SetSpeed();

        


        //If the player can move
        if (CanMove())
        {
            //Get the direction to move
            GetDirection(RB_InputManager.Instance.MoveValue);

            //Call the movement function
            Move();
        }
        

        //Call the dash function
        Dash();

        
    }


}
