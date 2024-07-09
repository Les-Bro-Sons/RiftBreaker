using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RB_AiMovement : MonoBehaviour
{
    [HideInInspector] public Vector3 WalkDirection;

    [SerializeField] private float _pushForce = 3f;

    [Header("IA movement properties (default, useless if set in a bt Tree)")]
    [HideInInspector] public Vector3 LastDirection;
    [SerializeField] private float _movementAcceleration; public float MovementAcceleration { get { return _movementAcceleration; } }
    [SerializeField] private float _movementMaxSpeed; public float MovementMaxSpeed { get { return _movementMaxSpeed; } }
    [SerializeField] private float _movementFrictionForce;
    public float MoveSpeedBoost = 1;

    [Header("Components")]
    private Rigidbody _rb;
    private Transform _transform;
    private RB_Health _health;
    private RB_AI_BTTree _btTree;

    [Header("Animation")]
    [SerializeField] private Animator _enemyAnimator;

    private NavMeshPath _navPath;

    private List<Rigidbody> _overlapBodies = new();

    private void Awake()
    {
        _rb = GetComponentInChildren<Rigidbody>();
        _transform = transform;
        _navPath = new NavMeshPath();
        _health = GetComponent<RB_Health>();
        _btTree = GetComponent<RB_AI_BTTree>();
    }

    private void FixedUpdate()
    {
        if (_rb && !_rb.isKinematic)
        {
            //Clamping the speed to the max speed
            ClampingSpeed();

            //Adding friction force
            FrictionForce();

            //add force to overlaping bodies
            PushOverlapingBodies();
        }
    }

    public void MoveIntoDirection(Vector3 direction, float? speed = null, float? acceleration = null, float? deltaTime = null) // deprecated
    {
        if (direction == Vector3.zero) return;
        if (speed == null) speed = _movementMaxSpeed;
        if (acceleration == null) acceleration = _movementAcceleration;
        if (deltaTime == null) deltaTime = Time.deltaTime;

        direction = direction.normalized;
        direction = RB_Tools.GetHorizontalDirection(direction);
        WalkDirection = direction.normalized;

        if (_rb.velocity.magnitude < _movementMaxSpeed)
        {
            //Adding velocity to player
            _rb.AddForce(direction * (speed.Value * MoveSpeedBoost) * deltaTime.Value * acceleration.Value);
        }
        _rb.MoveRotation(Quaternion.LookRotation(direction));
        LastDirection = direction;
    }

    public void MoveToPosition(Vector3 targetPos, float? speed = null, float? acceleration = null, float rotationSpeed = 4, float? deltaTime = null)
    {
        if (speed == null) speed = _movementMaxSpeed;
        if (acceleration == null) acceleration = _movementAcceleration;
        if (deltaTime == null) deltaTime = Time.deltaTime;

        if (NavMesh.CalculatePath(_transform.position, targetPos, NavMesh.AllAreas, _navPath))
        {
            if (_navPath.corners.Length <= 1) return; //1 because navpath sucks
            
            
            Vector3 nextPos = _navPath.corners[1];
            nextPos = new Vector3(nextPos.x, _transform.position.y, nextPos.z); //remove y change
            Vector3 direction = (nextPos - _transform.position).normalized; 
            direction = RB_Tools.GetHorizontalDirection(direction);
            WalkDirection = direction;

            _rb.AddForce(direction * (speed.Value * MoveSpeedBoost) * deltaTime.Value * acceleration.Value); //move
            _rb.MoveRotation(Quaternion.Lerp(_rb.rotation, Quaternion.LookRotation(direction), rotationSpeed * deltaTime.Value));

            LastDirection = direction;
        }
    }

    public NavMeshPath? GetPath(Vector3 targetPos)
    {
        NavMeshPath navMeshPath = new NavMeshPath();

        if (NavMesh.CalculatePath(_transform.position, targetPos, NavMesh.AllAreas, navMeshPath))
        {
            if (_navPath.corners.Length <= 1) return null; //1 because navpath sucks
            
            return navMeshPath;
        }
        else
        {
            return null;
        }
    }

    public float GetPathLength(NavMeshPath path)
    {
        float length = 0.0f;

        if ((path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length > 1))
        {
            for (int i = 1; i < path.corners.Length; ++i)
            {
                length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
        }

        return length;
    }

    public void RotateToward(Vector3 direction, float rotationSpeed = 4, float? deltaTime = null)
    {
        if (deltaTime == null) deltaTime = Time.deltaTime;

        direction = RB_Tools.GetHorizontalDirection(direction).normalized;

        _rb.MoveRotation(Quaternion.Lerp(_rb.rotation, Quaternion.LookRotation(direction), rotationSpeed * deltaTime.Value));
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

    private void PushOverlapingBodies()
    {
        foreach (Rigidbody body in _overlapBodies)
        {
            if (!body) _overlapBodies.Remove(body);
            body.AddForce(RB_Tools.GetHorizontalDirection(body.transform.position - transform.position) * _pushForce * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (RB_Tools.TryGetComponentInParent<RB_Health>(other.gameObject, out RB_Health collHealth) && collHealth.Team == _health.Team)
        {
            if (collHealth.TryGetComponent<Rigidbody>(out Rigidbody collRB))
            {
                _overlapBodies.Add(collRB);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (RB_Tools.TryGetComponentInParent<Rigidbody>(other.gameObject, out Rigidbody collRB) && _overlapBodies.Contains(collRB))
        {
            _overlapBodies.Remove(collRB);
        }
    }
}
