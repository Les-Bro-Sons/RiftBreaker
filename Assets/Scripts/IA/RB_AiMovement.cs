using UnityEngine;
using UnityEngine.AI;

public class RB_AiMovement : MonoBehaviour
{
    [HideInInspector] public Vector3 WalkDirection;

    [Header("IA movement properties")]
    [HideInInspector] public Vector3 LastDirection;
    [SerializeField] private float _movementAcceleration; public float MovementAcceleration { get { return _movementAcceleration; } }
    [SerializeField] private float _movementMaxSpeed; public float MovementMaxSpeed { get { return _movementMaxSpeed; } }
    [SerializeField] private float _movementFrictionForce;

    [Header("Components")]
    private Rigidbody _rb;
    private Transform _transform;

    [Header("Animation")]
    [SerializeField] private Animator _enemyAnimator;

    private NavMeshPath _navPath;

    private void Awake()
    {
        _rb = GetComponentInChildren<Rigidbody>();
        _transform = transform;
        _navPath = new NavMeshPath();
    }

    private void FixedUpdate()
    {
        //Clamping the speed to the max speed
        ClampingSpeed();

        //Adding friction force
        FrictionForce();
    }

    private void Update()
    {
        UpdateAnimator();
    }

    public void MoveIntoDirection(Vector3 direction, float speed = -1, float acceleration = -1, float deltaTime = -1) // deprecated
    {
        if (direction == Vector3.zero) return;
        if (speed == -1) speed = _movementMaxSpeed;
        if (acceleration == -1) acceleration = _movementAcceleration;
        direction = direction.normalized;
        WalkDirection = direction.normalized;

        if (deltaTime == -1)
        {
            deltaTime = Time.deltaTime;
        }

        if (_rb.velocity.magnitude < _movementMaxSpeed)
        {
            //Adding velocity to player
            _rb.AddForce(direction * speed * deltaTime * acceleration);
        }
        _transform.forward = direction;
        LastDirection = direction;
    }

    public void MoveToPosition(Vector3 targetPos, float speed = -1, float acceleration = -1, float deltaTime = -1)
    {
        if (speed == -1) speed = _movementMaxSpeed;
        if (acceleration == -1) acceleration = _movementAcceleration;
        if (deltaTime == -1) deltaTime = Time.deltaTime;

        if (NavMesh.CalculatePath(_transform.position, targetPos, NavMesh.AllAreas, _navPath))
        {
            if (_navPath.corners.Length <= 1) return; //1 because navpath sucks
            
            Vector3 nextPos = _navPath.corners[1];
            nextPos = new Vector3(nextPos.x, _transform.position.y, nextPos.z); //remove y change
            Vector3 direction = (nextPos - _transform.position).normalized;
            WalkDirection = direction;

            _rb.AddForce(direction * speed * deltaTime * acceleration); //move
            _rb.MoveRotation(Quaternion.LookRotation(direction));

            //_transform.forward = direction;
            LastDirection = direction;
        }
    }

    private void FrictionForce()
    {
        Vector3 frictionForce = (-_rb.velocity) * Time.fixedDeltaTime * _movementFrictionForce;
        _rb.AddForce(new Vector3(frictionForce.x, 0, frictionForce.z)); //Friction force (stop the movement)
    }

    private void ClampingSpeed()
    {
        //Clamping to max speed in the x and z axes
        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, _movementMaxSpeed);
        _rb.velocity = new Vector3(horizontalVelocity.x, _rb.velocity.y, horizontalVelocity.z);
    }

    private void UpdateAnimator()
    {
        //Updating the enemy animator
        if(_enemyAnimator != null)
        {
            _enemyAnimator.SetFloat("Horizontal", WalkDirection.x);
            _enemyAnimator.SetFloat("Vertical", WalkDirection.z);
            _enemyAnimator.SetFloat("Speed", _rb.velocity.magnitude);
        }
        
    }
}
