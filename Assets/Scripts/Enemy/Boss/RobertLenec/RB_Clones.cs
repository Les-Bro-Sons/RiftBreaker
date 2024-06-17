using UnityEngine;
using UnityEngine.Rendering.UI;

public class RB_Clones : RB_Enemy
{

    public GameObject RedBall;
    [HideInInspector] public float Lifetime;
    private float _lifetimeTimer = 0;
    [SerializeField] private float _redBallOffset = 1f;
    [SerializeField] private float _shootCooldown = 0.25f;
    [SerializeField] private float _movingDuration = 0.65f;
    private float _movingTimer = 0f;
    protected float _currentCooldown;
    public Vector3 TargetPosition;
    private Vector3 _startPosition;

    private bool _movingToShoot = false;
    private bool _movingToBoss = false;

    [SerializeField] private ParticleSystem _moveParticles;

    protected override void Awake()
    {
        base.Awake();
        _startPosition = transform.position;
        _movingToShoot = true;
        if (_moveParticles) _moveParticles.Play();
    }

    private void Update()
    {
        if (_movingToShoot)
        {
            if (_movingTimer >= _movingDuration)
            {
                if (_moveParticles) _moveParticles.Stop();
                transform.position = TargetPosition;
                _currentCooldown -= Time.deltaTime;
                GetTarget();
                if (_currentCooldown <= 0)
                {
                    CloneShoot();
                }
                _movingToShoot = false;
            }
            else
            {
                _rb.MovePosition(Vector3.Lerp(_startPosition, TargetPosition, _movingTimer / _movingDuration));
                _rb.MoveRotation(Quaternion.Euler(transform.TransformDirection(TargetPosition - _startPosition)));
                _movingTimer += Time.deltaTime;
            }
        }
        else if (_movingToBoss)
        {
            if (_movingTimer >= _movingDuration)
            {
                if (_moveParticles) _moveParticles.Stop();
                _movingToBoss = false;
                _moveParticles.transform.parent = null;
                Destroy(_moveParticles.gameObject, 5);
                Destroy(gameObject);
                return;
            }
            else
            {
                _rb.MovePosition(Vector3.Lerp(TargetPosition, _startPosition, _movingTimer / _movingDuration));
                _movingTimer += Time.deltaTime;
            }
        }

        _lifetimeTimer += Time.deltaTime;
        if (_lifetimeTimer + _movingTimer >= Lifetime && !_movingToBoss)
        {
            _movingToShoot = false;
            _movingToBoss = true;
            _movingTimer = 0;
            if (_moveParticles) _moveParticles.Play();
        }
    }

    public void CloneShoot()
    {
        transform.forward = _currentTarget.position - transform.position;
        Vector3 offset = transform.forward * _redBallOffset;
        Vector3 spawnProjectile = transform.position + offset;
        Instantiate(RedBall, spawnProjectile, transform.rotation);

        _currentCooldown = _shootCooldown;
    }
}
