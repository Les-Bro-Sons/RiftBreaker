using Cinemachine;
using System.Collections.Generic;
using MANAGERS;
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
    [SerializeField] private float _speed; public float Speed { get { return _speed; } set { _speed = value; } }
    [SerializeField] private float _totalDistance; public float TotalDistance { get { return _totalDistance; } set { _totalDistance = value; } }
    [SerializeField] private ProjectileType _projectileType;
    [SerializeField] private Vector3 _launchForce;
    [SerializeField] private float _totalLifeTime;
    [SerializeField] public TEAMS Team;
    [SerializeField] private bool _destroyOnWall;
    [SerializeField] private float _wallDetectionLength = 1;

    [Header("Particles")]
    [SerializeField] private GameObject _followParticles;
    [SerializeField] private GameObject _destroyParticles;

    [Header("Screenshake")]
    [SerializeField] private float _continuousForceScreenshake;
    [SerializeField] private float _continuousDelayScreenshake;
    private float _currentContinousDelayScreenshake = 9999; // set at 9999 so it impulse at the start

    [Header("Damage")]
    [SerializeField] private float _damage = 10; public float Damage { get { return _damage; } set { _damage = value; } }
    [SerializeField] private float _knocbackExplosionForce = 0; public float KnocbackExplosionForce { get {  return _knocbackExplosionForce; } set { _knocbackExplosionForce = value; } }
    [SerializeField] private Vector3 _knockback = Vector3.zero;
    [SerializeField] private bool _isDealingDamageMultipleTime = false;
    [SerializeField] private bool _isDealingKnockbackMultipleTime = false;
    [SerializeField] private bool _canDamageAlly = false;
    [SerializeField] private bool _canKnockbackAlly = false;
    [SerializeField] private bool _isDestroyingOnDamage = false;
    private List<GameObject> _alreadyDamaged = new();

    [Header("Explosion")]
    [SerializeField] private bool _damageOnExplosion = false;
    [SerializeField] private float _explosionRadius = 1;

    [Header("Sounds")] 
    [SerializeField] private string _explosionSounds;
    
    //Components
    private Rigidbody _rb;
    private Transform _transform;
    private CinemachineImpulseSource _impulseSource;
    [SerializeField] private Animator _projectileAnimator;

    //Movements
    private float _traveledDistance;
    private Vector3 _firstPos;
    private float _creationTime;

    //Spawn prefab
    public float SpawnDistanceFromPlayer;


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
        if (_damageOnExplosion)
        {
            Explode();
            return;
        }

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
            if (_isDestroyingOnDamage)
            {
                if (_destroyParticles)
                    Instantiate(_destroyParticles, _transform.position, _transform.rotation);
                DestroyParticle();
            }
        }
    }

    private void Explode()
    {
        foreach (Collider collider in Physics.OverlapSphere(_transform.position, _explosionRadius))
        {
            if (RB_Tools.TryGetComponentInParent<RB_Health>(collider.gameObject, out RB_Health enemyHealth))
            {
                enemyHealth.TakeKnockback(_transform.TransformDirection(_knockback.normalized), _knockback.magnitude);
                enemyHealth.TakeKnockback(collider.transform.position - _transform.position, _knocbackExplosionForce);
                enemyHealth.TakeDamage(_damage);
            }
        }
        if (_destroyParticles)
            Instantiate(_destroyParticles, _transform.position, _transform.rotation);
        RB_AudioManager.Instance.PlaySFX(_explosionSounds, transform.position,0, 1);
        DestroyParticle();
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
        if (_destroyOnWall && Physics.Raycast(_transform.position, _rb.velocity.normalized, _wallDetectionLength, 1 << 3))
        {
            if (_damageOnExplosion)
            {
                Explode();
            }
            else
            {
                if (_destroyParticles)
                    Instantiate(_destroyParticles, _transform.position, _transform.rotation);
                DestroyParticle();
            }
            
        }
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
                DestroyParticle();
            }
        }
        else
        {
            if(Time.time > (_creationTime + _totalLifeTime))
            {
                //If the projectile is meant to be launched, when its life time is finished, destroy it
                if (_destroyParticles)
                    Instantiate(_destroyParticles, _transform.position, _transform.rotation);
                DestroyParticle();
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

        UpdateAnim();
    }

    private void UpdateAnim()
    {
        if(_projectileAnimator != null)
        {
            _projectileAnimator.SetFloat("Horizontal", _transform.TransformDirection(Vector3.forward).x);
            _projectileAnimator.SetFloat("Vertical", _transform.TransformDirection(Vector3.forward).z);
        }
    }

    private void DestroyParticle() 
    {
        RB_AudioManager.Instance.PlaySFX(_explosionSounds, transform.position,0, .1f);
        Destroy(gameObject);
    }
}
