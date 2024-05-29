using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_Sylvashot : RB_Boss
{
    public BOSSSTATES CurrentState = BOSSSTATES.Idle;

    [Header("Slash (attack1)")]
    [SerializeField] private float _slashDamage = 30;
    [SerializeField] private float _slashKnockback = 15;
    [SerializeField] private float _slashRange = 3;
    [SerializeField] private float _slashDelay = 0.5f;
    [SerializeField] private GameObject _slashParticles;
    private float _slashDelayTimer = 0;

    [Header("Single Shot (attack1)")]
    public GameObject PieceOfWood;
    [SerializeField] private float _minMovementDistance = 3f;
    [SerializeField] private float _maxMovementDistance = 10f;
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _shootDelay = 0.5f;
    [SerializeField] private float _pieceOfWoodSpeed = 0.75f;
    [SerializeField] private float _pieceOfWoodDamage = 10;
    [SerializeField] private float _pieceOfWoodKnockback = 10;
    [SerializeField] private bool _canPieceOfWoodDamageMultipleTime = false;
    private List<RB_Health> _alreadyPieceOfWoodDamaged = new();
    private float _shootDelayTimer = 0;

    [Header("WoodenPiece Rain Zone (attack2)")]
    public GameObject WoodenPieceRainZone;
    [SerializeField] private float _areaDamageInterval = 1f;
    [SerializeField] private float _areaDamageRadius = 1f;
    [SerializeField] private float _areaDamageAmount = 1f;
    [SerializeField] private float _areaDamageDuration = 1f;
    private float _lastAreaDamageTime;

    [Header("Clone Attack (attack3)")]
    public GameObject Clone;
    [SerializeField] private int _numberOfArrow;
    [SerializeField] private float _cloneAttackInterval = 0.5f;
    [SerializeField] private float _cloneAttackDelay = 1f;
    private List<GameObject> clones = new List<GameObject>();

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        _currentWaitInIdle -= Time.deltaTime;
        _currentCooldownBetweenAttacks -= Time.deltaTime;
        _currentCooldownAttack1 -= Time.deltaTime;
        _currentCooldownAttack2 -= Time.deltaTime;
        _currentCooldownAttack3 -= Time.deltaTime;
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
                _movement.MoveIntoDirection(_currentTarget.position - transform.position); //move to the target
                SwitchBossState();
                break;
            case BOSSSTATES.Attack1:
               //if (WaitForSlash())
               //{
               //    //Slash();
               //    SwitchBossState();
               //}
                break;
            case BOSSSTATES.Attack2:
                _currentWaitInIdle = WaitInIdle;
                CurrentState = BOSSSTATES.Idle; //wait in idle
                break;
            case BOSSSTATES.Attack3:
                break;
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
            if (_currentCooldownAttack1 <= 0 && GetTargetDistance() <= 3f) //SWITCH TO ATTACK1
            {
                transform.forward = (_currentTarget.position - transform.position).normalized;
                _slashDelayTimer = _slashDelay;
                return CurrentState = BOSSSTATES.Attack1;
            }
        
            if (_currentCooldownAttack2 <= 0 && GetTargetDistance() <= 7f && GetTargetDistance() >= 5f) //SWITCH TO ATTACK2
            {
                //_alreadySpikeDamaged.Clear();
                //KickAttack();
                return CurrentState = BOSSSTATES.Attack2;
            }
        
            if (_currentCooldownAttack3 <= 0 && GetTargetDistance() > 7f) //SWITCH TO ATTACK3
            {
                //StartJumpAttack();
                return CurrentState = BOSSSTATES.Attack3;
            }
        }


        //if (MovementSpeed >= 0.1f) //SWITCH TO MOVING
        //{
        return CurrentState = BOSSSTATES.Moving;
        //}
        //SWITCH TO IDLE

        _currentWaitInIdle = WaitInIdle;
        return BOSSSTATES.Idle;
    }

    private bool WaitForSlash() //TIMER ATTACK 1
    {
        _slashDelayTimer -= Time.fixedDeltaTime;
        return (_slashDelayTimer <= 0);
    }

    public void MovementBeforeShoot() //1ST PART OF ATTACK 1
    {
        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0;
        randomDirection.Normalize();

        float randomDistance = Random.Range(_minMovementDistance, _maxMovementDistance);

        Vector3 destination = transform.position + randomDirection * randomDistance;

        _movement.MoveIntoDirection(destination - transform.position);
    }
    private bool WaitForShoot() //TIMER BETWEEN MOVEMENT AND SHOOT INTO ATTACK 1
    {
        _shootDelayTimer -= Time.fixedDeltaTime;
        return (_shootDelayTimer <= 0);
    }
    public void SingleShotAttack() //2ND PART OF ATTACK 1
    {
        WaitForShoot();
        Vector3 directionToPlayer = (_currentTarget.position - transform.position).normalized;
        GameObject projectile = Instantiate(PieceOfWood, transform.position, Quaternion.identity);
        RB_Projectile projectileScript = projectile.GetComponent<RB_Projectile>();

        Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
        if (projectileRigidbody != null)
        {
            projectileRigidbody.velocity = directionToPlayer * _pieceOfWoodSpeed;
        }

        _currentCooldownAttack2 = CooldownAttack2;
    }

    public void WoodenPieceRainZoneAttack() //ATTACK 2
    {
        if (Time.time - _lastAreaDamageTime >= _areaDamageInterval)
        {
            _lastAreaDamageTime = Time.time;
            GameObject areaDamageInstance = Instantiate(WoodenPieceRainZone, _currentTarget.position, Quaternion.identity);
            areaDamageInstance.transform.localScale = new Vector3(_areaDamageRadius * 2, areaDamageInstance.transform.localScale.y, _areaDamageRadius * 2);

            Collider[] colliders = Physics.OverlapSphere(transform.position, _areaDamageRadius, PlayerLayer);
            foreach (Collider hitCollider in colliders)
            {
                RB_PlayerController player = hitCollider.GetComponent<RB_PlayerController>();
                if (player != null)
                {
                    Health.TakeDamage(_areaDamageAmount);
                }
            }

            Destroy(areaDamageInstance, _areaDamageDuration);
        }
    }

    public void CloneAttack()
    {

    }

    public void ApplyPieceOfWoodDamage(RB_Health enemyHealth)
    {
        if (Health.Team == enemyHealth.Team || (_alreadyPieceOfWoodDamaged.Contains(enemyHealth) && !_canPieceOfWoodDamageMultipleTime)) return;

        _alreadyPieceOfWoodDamaged.Add(enemyHealth);
        enemyHealth.TakeDamage(_pieceOfWoodDamage);
        enemyHealth.TakeKnockback((enemyHealth.transform.position - transform.position).normalized, _pieceOfWoodKnockback);
    }   
}
