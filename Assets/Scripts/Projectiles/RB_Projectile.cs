using Cinemachine;
using System.Collections.Generic;
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
    [SerializeField] public TEAMS Team;

    [Header("Particles")]
    [SerializeField] private GameObject _followParticles;
    [SerializeField] private GameObject _destroyParticles;

    [Header("Screenshake")]
    [SerializeField] private float _continuousForceScreenshake;
    [SerializeField] private float _continuousDelayScreenshake;
    private float _currentContinousDelayScreenshake = 9999; // set at 9999 so it impulse at the start

    [Header("Damage")]
    [SerializeField] private float _damage = 10;
    [SerializeField] private float _knocbackExplosionForce = 0;
    [SerializeField] private Vector3 _knockback = Vector3.zero;
    [SerializeField] private bool _isDealingDamageMultipleTime = false;
    [SerializeField] private bool _isDealingKnockbackMultipleTime = false;
    [SerializeField] private bool _canDamageAlly = false;
    [SerializeField] private bool _canKnockbackAlly = false;
    private List<GameObject> _alreadyDamaged = new(); 

    //Components
    private Rigidbody _rb;
    private Transform _transform;
    private CinemachineImpulseSource _impulseSource;

    //Movements
    private float _traveledDistance;
    private Vector3 _firstPos;
    private float _creationTime;


    private void Awake()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody>();
        RB_CollisionDetection collision = GetComponent<RB_CollisionDetection>();
        if (collision)
        {
            collision.EventOnEnemyEntered.AddListener(delegate { EnemyEntered(collision.GetDetectedObjects()[collision.GetDetectedObjects().Count-1]); });
        }

        if (_followParticles)
            Instantiate(_followParticles, _transform.position, _transform.rotation).GetComponent<RB_Particles>().FollowObject = _transform;

        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void EnemyEntered(GameObject enemy)
    {
        bool isAlreadyDamaged = true;
        if (!_alreadyDamaged.Contains(enemy))
        {
            _alreadyDamaged.Add(enemy);
            isAlreadyDamaged = false;
        }
        RB_Health enemyHealth = enemy.GetComponent<RB_Health>();

        bool isAlly = (enemyHealth.Team == Team);

        if ((_isDealingKnockbackMultipleTime || !isAlreadyDamaged) && (_canKnockbackAlly || !isAlly)) // if it isn't already damaged or can damage multiple time
        {
            enemyHealth.TakeKnockback(_transform.TransformDirection(_knockback.normalized), _knockback.magnitude);
            enemyHealth.TakeKnockback(enemy.transform.position - _transform.position, _knocbackExplosionForce);
        }
        if ((_isDealingDamageMultipleTime || !isAlreadyDamaged) && (_canDamageAlly || !isAlly)) // if it isn't already damaged or can damage multiple time
        {
            enemyHealth.TakeDamage(_damage);
        }
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
                if (_destroyParticles)
                    Instantiate(_destroyParticles, _transform.position, _transform.rotation);
                Destroy(gameObject);
            }
        }
        else
        {
            if(Time.time > (_creationTime + _totalLifeTime))
            {
                //If the projectile is meant to be launched, when its life time is finished, destroy it
                if (_destroyParticles)
                    Instantiate(_destroyParticles, _transform.position, _transform.rotation);
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

    private void Update()
    {
        if (_continuousForceScreenshake != 0) //continuous screenshake
        {
            if (_currentContinousDelayScreenshake >= _continuousDelayScreenshake + _impulseSource.m_ImpulseDefinition.m_ImpulseDuration) //apply screenshake if the delay is met
            {
                _impulseSource.GenerateImpulse(_continuousForceScreenshake);
                _currentContinousDelayScreenshake = 0;
            }

            _currentContinousDelayScreenshake += Time.deltaTime;
        }
    }
}
