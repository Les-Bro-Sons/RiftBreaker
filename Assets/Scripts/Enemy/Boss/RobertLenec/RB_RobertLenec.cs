using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RB_RobertLenec : RB_Boss
{
    public static RB_RobertLenec Instance;
    private new Transform transform;

    public BOSSSTATES CurrentState = BOSSSTATES.Idle;

    [Header("Movement")]
    [SerializeField] private float _minMovementDistance = 3f;
    [SerializeField] private float _maxMovementDistance = 10f;
    [SerializeField] private float _movementSpeed = 10f;
    [SerializeField] private float _delayMovement = 1f;
    protected float _currentCooldownBetweenMovement;

    [Header("Single Shot (attack1)")]
    public GameObject RedBall;
    [SerializeField] private float _minMovementBeforeAttackDistance = 3f;
    [SerializeField] private float _maxMovementBeforeAttackDistance = 10f;
    [SerializeField] private float _dashBeforeAttackSpeed = 10f;
    [SerializeField] private float _redBallOffset = 1f;

    [Header("WoodenPiece Rain Zone (attack2)")]
    public GameObject WoodenPieceRainZone;
    [SerializeField] private float _areaDamageRadius = 1f;
    [SerializeField] private float _areaDamageInterval = 1f;
    [SerializeField] private float _areaDamageAmount = 1f;
    [SerializeField] private float _areaDamageKnockback = 1f;
    [SerializeField] private bool _canAreaDamageZoneDamageMultipleTime = false;
    protected float _currentCooldownBeforeTakeDamage;
    private List<RB_Health> _alreadyAreaDamageZoneDamaged = new();

    [Header("Clone Attack (attack3)")]
    public GameObject Clone;
    [SerializeField] private List<Transform> _waypoints;
    [SerializeField] private int _numberOfArrow;
    [SerializeField] private float _cloneAttackInterval = 0.5f;
    [SerializeField] private float _cloneAttackDelay = 1f;
    [SerializeField] private float _cloneLifeTime = 1f;
    [SerializeField] private float _cooldownForReaparition = 1f;
    [SerializeField] private float _minCooldownForAttack = 10f;
    [SerializeField] private float _maxCooldownForAttack = 30f;
    private List<GameObject> _clones = new List<GameObject>();
    private Vector3 _lastPosition;
    protected float _currentCooldownBeforeReactivate;

    protected override void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
        base.Awake();
        transform = GetComponent<Transform>();
    }

    protected override void Start()
    {
        base.Start();
        WoodenPieceRainZoneAttack();
    }

    protected override void Update()
    {
        _currentWaitInIdle -= Time.deltaTime;
        _currentCooldownBetweenAttacks -= Time.deltaTime;
        _currentCooldownAttack1 -= Time.deltaTime;
        _currentCooldownAttack2 -= Time.deltaTime;
        _currentCooldownAttack3 -= Time.deltaTime;
        _currentCooldownBetweenMovement -= Time.deltaTime;
        _currentCooldownBeforeTakeDamage -= Time.deltaTime;
        _currentCooldownBeforeReactivate -= Time.deltaTime;
        //_mouvement.MoveIntoDirection(PlayerPosition.position);
        
    }

    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case BOSSSTATES.Idle:
                if (_currentWaitInIdle <= 0) //when waited enough in idle, switch state
                {
                    SwitchBossState();
                }
                break;
            case BOSSSTATES.Moving:
                //transform.forward = (_currentTarget.position - transform.position).normalized;
                Movement();
                SwitchBossState();
                break;
            case BOSSSTATES.Attack1:
                SwitchBossState();
                break;
            case BOSSSTATES.Attack2:
                _currentWaitInIdle = WaitInIdle;
                CurrentState = BOSSSTATES.Idle; //wait in idle
                break;
            case BOSSSTATES.Attack3:
                _currentWaitInIdle = WaitInIdle;
                CurrentState = BOSSSTATES.Idle;
                break;
        }
        if (_currentCooldownBeforeTakeDamage <= 0)
        {
            _alreadyAreaDamageZoneDamaged.Clear();
        }
    }

    private BOSSSTATES SwitchBossState()
    {
        GetTarget();

        switch (CurrentState) //Action depending on what the state machine is switching state from
        {
            case BOSSSTATES.Attack1:
            case BOSSSTATES.Attack2:
            case BOSSSTATES.Attack3:
                _currentCooldownBetweenAttacks = CooldownBetweenAttacks;
                break;
            default:
                break;
        }

        if (_currentCooldownBetweenAttacks <= 0)
        {
            if (_currentCooldownAttack3 <= 0) //SWITCH TO ATTACK3
            {
                CloneAttack();
                return CurrentState = BOSSSTATES.Attack3;
            }

            if (_currentCooldownAttack1 <= 0 && GetTargetDistance() > 5f) //SWITCH TO ATTACK1
            {
                ShootAttack();
                return CurrentState = BOSSSTATES.Attack1;
            }
        
            if (_currentCooldownAttack2 <= 0 && GetTargetDistance() < 5f && GetTargetDistance() > 2f) //SWITCH TO ATTACK2
            {
                WoodenPieceRainZoneAttack();
                return CurrentState = BOSSSTATES.Attack2;
            }
            
            if (_currentCooldownBetweenMovement <= 0)
            {
                RandomMovement();
                return CurrentState = BOSSSTATES.Moving;
            }
        }
        
        return CurrentState = BOSSSTATES.Moving;
    }

    public void Movement()
    {
        //Boss goes to the player if he's too far away
        if (GetTargetDistance() > 10f)
        {
            _movement.MoveIntoDirection(_currentTarget.position - transform.position);
        }

        //Boss goes away the player if he's too far away
        else if (GetTargetDistance() < 5f)
        {
            _movement.MoveIntoDirection(transform.position - _currentTarget.position);
        }

    }

    public void RandomMovement()
    {
        //Boss do random movement when he's not too close or too far from the player
        if (GetTargetDistance() <= 10f && GetTargetDistance() >= 5f)
        {
            if (_currentCooldownBetweenMovement <= 0)
            {
                Vector3 randomDirection = Random.insideUnitSphere;
                randomDirection.y = 0;
                randomDirection.Normalize();

                float randomDistance = Random.Range(_minMovementDistance, _maxMovementDistance);

                Vector3 destination = transform.position + randomDirection * randomDistance;

                _movement.MoveIntoDirection(destination, _movementSpeed);
                transform.forward = (_currentTarget.position - transform.position).normalized;

            }
            _currentCooldownBetweenMovement = _delayMovement;
        }
    }

    public void ShootAttack() //ATTACK 1
    {
        //Dash Into a Random Direction
        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0;
        randomDirection.Normalize();

        float randomDistance = Random.Range(_minMovementBeforeAttackDistance, _maxMovementBeforeAttackDistance);

        Vector3 destination = transform.position + randomDirection * randomDistance;

        _movement.MoveIntoDirection(destination, _dashBeforeAttackSpeed);

        //Spawn of projectiles (attack 1)
        transform.forward = _currentTarget.position - transform.position;
        Vector3 offset = transform.forward * _redBallOffset;
        Vector3 spawnProjectile = transform.position + offset;
        Instantiate(RedBall, spawnProjectile, transform.rotation);

        _currentCooldownAttack1 = CooldownAttack1;
    }

    public void WoodenPieceRainZoneAttack() //ATTACK 2
    {
        //Spawn of the zone attack (attack n°2)
        float offset = 0.99f;
        Vector3 areaDamageSpawn = new Vector3(_currentTarget.position.x, _currentTarget.position.y - offset, _currentTarget.position.z);
        RB_RainZone rainZone = Instantiate(WoodenPieceRainZone, areaDamageSpawn, Quaternion.identity).GetComponent<RB_RainZone>();
        rainZone.Sylvashot = this;
        rainZone.DamageCooldown = _areaDamageInterval;
        WoodenPieceRainZone.transform.localScale = new Vector3(_areaDamageRadius * 2, WoodenPieceRainZone.transform.localScale.y, _areaDamageRadius * 2);
        _currentCooldownAttack2 = CooldownAttack2;
    }

    public void ApplyRainZoneDamage(List<RB_Health> enemyHealths)
    {
        //Application of damages for the zone attack (attack 2)
        foreach (RB_Health enemyHealth in enemyHealths)
        {
            if (Health.Team == enemyHealth.Team || (_alreadyAreaDamageZoneDamaged.Contains(enemyHealth) && !_canAreaDamageZoneDamageMultipleTime)) continue;
            _alreadyAreaDamageZoneDamaged.Add(enemyHealth);
            enemyHealth.TakeDamage(_areaDamageAmount);
        }
        
        //Cooldown
        //_currentCooldownBeforeTakeDamage = _areaDamageInterval;
    }
    public void CloneAttack()
    {
        //Instantiation of clones
        for (int i = 0; i < 4; i++)
        {
            GameObject clone = Instantiate(Clone, transform.position, Quaternion.identity);
            RB_Clones cloneScript = clone.GetComponent<RB_Clones>();
            cloneScript.TargetPosition = _waypoints[i].position;
            cloneScript.Lifetime = _cloneLifeTime;
        }

        //get the position of the boss before the attack
        _lastPosition = transform.position;
        //tp the boss out of the map
        transform.position = new Vector3(transform.position.x + 3000, transform.position.y, transform.position.z);

        //Cooldown
        _currentCooldownAttack3 = Random.Range(_minCooldownForAttack, _maxCooldownForAttack);

        StartCoroutine(ReturnToLastPos(_cloneLifeTime));
    }

    IEnumerator ReturnToLastPos(float duration)
    {
        yield return new WaitForSeconds(duration);

        _rb.MovePosition(_lastPosition);
    }
}
