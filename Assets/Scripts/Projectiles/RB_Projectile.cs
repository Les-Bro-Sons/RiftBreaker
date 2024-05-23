using UnityEngine;

public class RB_Projectile : MonoBehaviour
{
    //Enum
    public enum ProjectileType
    {
        launch, linear
    }

    //Properties
    [Header("Properties")]
    [SerializeField] private float _speed;
    [SerializeField] private float _totalDistance;
    [SerializeField] private ProjectileType _projectileType;
    [SerializeField] private Vector3 _launchForce;
    [SerializeField] private float _totalLifeTime;

    //Components
    private Rigidbody _rb;
    private Transform _transform;

    //Movements
    private float _traveledDistance;
    private Vector3 _firstPos;
    private float _creationTime;


    private void Awake()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //If the projectile is meant to be launched
        if(_projectileType==ProjectileType.launch)
        {
            //Launch the projectile
            Launch();
        }
        //Set the first position of the projectile
        _firstPos = _transform.position;
        //And the time of his creation
        _creationTime = Time.time;
    }

    private void FixedUpdate()
    {
        //Move the projectile
        MoveLinear();
    }

    private void MoveLinear()
    {
        //If the projectile is a linear one
        if(_projectileType == ProjectileType.linear)
        {
            _traveledDistance = Vector3.Distance(_firstPos, _transform.position);
            if (_traveledDistance < _totalDistance)
            {
                //While the distance traveled is less than the total distance wanted
                _rb.velocity = _transform.forward * _speed;
            }
            else
            {
                //When it reaches the total distance, destroy the projectile
                Destroy(gameObject);
            }
        }
        else
        {
            if(Time.time > (_creationTime + _totalLifeTime))
            {
                //If the projectile is meant to be launched, when its life time is finished, destroy it
                Destroy(gameObject);
            }
        }
        
    }

    private void Launch()
    {
        //Launch the projectile
        _rb.constraints = ~RigidbodyConstraints.FreezePosition;
        _rb.AddForce(_transform.TransformDirection(_launchForce), ForceMode.Impulse);
    }
}
