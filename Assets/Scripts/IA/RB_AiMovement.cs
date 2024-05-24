using UnityEngine;

public class RB_AiMovement : MonoBehaviour
{

    [Header("IA movement properties")]
    [HideInInspector] public Vector3 LastDirection;
    [SerializeField] private float _movementAcceleration;
    [SerializeField] private float _movementMaxSpeed;
    [SerializeField] private float _movementFrictionForce;

    [Header("Components")]
    private Rigidbody _rb;
    private Transform _transform;

    private void Awake()
    {
        _rb = GetComponentInChildren<Rigidbody>();
        _transform = transform;
    }

    private void FixedUpdate()
    {
        //Clamping the speed to the max speed
        ClampingSpeed();

        //Adding friction force
        FrictionForce();
    }

    public void MoveIntoDirection(Vector3 direction, float speed = -1, float acceleration = -1, float deltaTime = -1)
    {
        if (direction == Vector3.zero) return;
        if (speed == -1) speed = _movementMaxSpeed;
        if (acceleration == -1) acceleration = _movementAcceleration;
        direction = direction.normalized;

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
}
