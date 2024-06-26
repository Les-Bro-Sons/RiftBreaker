using MANAGERS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RB_RobertLenec : RB_Boss
{
    public static RB_RobertLenec Instance;
    [HideInInspector] public UnityEvent EventPlayRobertMusic;
    private bool _inRoom = false;
    public BOSSSTATES CurrentState = BOSSSTATES.Idle;

    private float _activationTimer = 0;

    [Header("Movement")]
    [SerializeField] private float _minMovementDistance = 3f;
    [SerializeField] private float _maxMovementDistance = 10f;
    [SerializeField] private float _movementSpeed = 10f;
    [SerializeField] private float _delayMovement = 1f;
    [SerializeField] private float _distanceBehind = 1f;
    [SerializeField] private float _movingDuration = 1f;
    [SerializeField] private float _dashKnockback = 1f;
    private float _movingTimer = 0f;
    private Vector3 positionBehindPlayer;
    [SerializeField] private float _tooCloseTime = 3;
    private float _tooCloseTimer = 0;
    protected float _currentCooldownBetweenMovement;
    private LayerMask _layerMask;


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
    [SerializeField] private Transform _tpPoint;
    [SerializeField] private int _numberOfArrow;
    [SerializeField] private float _cloneAttackInterval = 0.5f;
    [SerializeField] private float _cloneAttackDelay = 1f;
    [SerializeField] private float _cloneLifeTime = 1f;
    [SerializeField] private float _cooldownForReaparition = 1f;
    [SerializeField] private float _minCooldownForAttack = 10f;
    [SerializeField] private float _maxCooldownForAttack = 30f;
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
        _layerMask = 3;
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
        if (Health.Dead) return;

        int? bossRoom = RB_RoomManager.Instance.GetEntityRoom(Health.Team, gameObject);
        int? playerRoom = RB_RoomManager.Instance.GetPlayerCurrentRoom();
        Room();
        
        if (bossRoom == null || playerRoom == null || (bossRoom.Value != playerRoom.Value))
        {
            _activationTimer = 0;
            return;
        }
        else if (_activationTimer < 0.5f)
        {
            CurrentState = BOSSSTATES.Idle;
            _activationTimer += Time.deltaTime;
            return;
        }
        GetTarget();

        Repositionning();
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
                SwitchBossState();
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
            
            /*if (_currentCooldownBetweenMovement <= 0)
            {
                RandomMovement();
                return CurrentState = BOSSSTATES.Moving;
            }*/
        }
        
        return CurrentState = BOSSSTATES.Moving;
    }

    private void Room()
    {

        if (_inRoom == false)
        {
            EventPlayRobertMusic.Invoke();
            _inRoom = true;
        }
        if (_inRoom == true)
        {
            return;
        }
    }
    public void Movement()
    {
        //Boss goes to the player if he's too far away
        if (GetTargetDistance() > 10f)
        {
            _movement.MoveIntoDirection(_currentTarget.position - transform.position);
        }

        //Boss goes away the player if he's too far away
        else if (GetTargetDistance() < 5f && _tooCloseTimer < _tooCloseTime)
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
        AiAnimator.SetTrigger("BasicZone");
        RB_AudioManager.Instance.PlaySFX("Magic_Ball_Sound",transform.position, false, 0f, 1f);
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
        AiAnimator.SetTrigger("BasicZone");
        float offset = 0.99f;
        Vector3 areaDamageSpawn = new Vector3(_currentTarget.position.x, _currentTarget.position.y - offset, _currentTarget.position.z);
        RB_RainZone rainZone = Instantiate(WoodenPieceRainZone, areaDamageSpawn, Quaternion.identity).GetComponent<RB_RainZone>();
        rainZone.Sylvashot = this;
        rainZone.DamageCooldown = _areaDamageInterval;
        rainZone.transform.localScale = new Vector3(_areaDamageRadius * 2, WoodenPieceRainZone.transform.localScale.y, _areaDamageRadius * 2);
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
        AiAnimator.SetTrigger("Clonage");
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
        transform.position = _tpPoint.position;

        //Cooldown
        _currentCooldownAttack3 = Random.Range(_minCooldownForAttack, _maxCooldownForAttack);
        StartCoroutine(ReturnToLastPos(_cloneLifeTime));
    }

    IEnumerator ReturnToLastPos(float duration)
    {
        yield return new WaitForSeconds(duration);

        _rb.MovePosition(_lastPosition);
    }

    public void Repositionning()
    {

        if (GetTargetDistance() < 4f)
        {
            _tooCloseTimer += Time.deltaTime;
            if (_tooCloseTimer <= _tooCloseTime)
            {
                positionBehindPlayer = Vector3.zero;
            }  
            else
            {
                if (_movingTimer <= _movingDuration)
                {
                    if (positionBehindPlayer == Vector3.zero)
                    {
                        positionBehindPlayer = (_currentTarget.position - transform.position).normalized * _distanceBehind;
                    }

                    foreach (Collider enemy in Physics.OverlapBox(transform.position, Vector3.one, transform.rotation))
                    {
                        if (RB_Tools.TryGetComponentInParent<RB_Health>(enemy.gameObject, out RB_Health enemyHealth))
                        {
                            enemyHealth.TakeKnockback(enemyHealth.transform.position - transform.position, _dashKnockback);
                        }
                    }

                    Vector3 startPosition = transform.position;

                    Vector3 dashDirection = positionBehindPlayer - startPosition;
                    float dashDistance = dashDirection.magnitude;


                    if (Physics.Raycast(startPosition, dashDirection.normalized, out RaycastHit hit, dashDistance, 1 << 3))
                    {
                        positionBehindPlayer = hit.point;
                    }
                    _rb.MovePosition(Vector3.Lerp(startPosition, positionBehindPlayer, _movingTimer / _movingDuration));

                    _movingTimer += Time.deltaTime;

                    if (_movingTimer >= _movingDuration)
                    {
                        _tooCloseTimer = 0;
                        
                    }
                }
            }
        }
        else
        { 
            _tooCloseTimer = 0f;
            _movingTimer = 0;
            positionBehindPlayer = Vector3.zero;
        }
    }
}

