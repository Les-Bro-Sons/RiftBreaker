using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RB_Yog : RB_Boss
{
    public BOSSSTATES CurrentState = BOSSSTATES.Idle;

    [Header("Movement")]
    [SerializeField] private float _minMovementDistance = 3f;
    [SerializeField] private float _maxMovementDistance = 10f;
    [SerializeField] private float _movementSpeed = 10f;
    [SerializeField] private float _delayMovement = 1f;
    [SerializeField] private float _timeForMoving = 6f;
    private float _timeUntilNextState;
    protected float _currentCooldownBetweenMovement;

    [Header("Tentacle Hit (attack1)")]
    [SerializeField] private float _tentacleHitWidth = 1f;
    [SerializeField] private float _tentacleHitLength = 1f;
    [SerializeField] private float _tentacleHitRange = 1f;
    [SerializeField] private float _tentacleHitKnockback = 1f;
    [SerializeField] private float _tentacleHitDamage = 1f;
    [SerializeField] private float _tentacleHitDelay = 1f;
    [SerializeField] private int _numberTotalOfAttack = 5;
    [SerializeField] private GameObject _tentacleHitParticles;
    [SerializeField] private List<int> _numberOfAttackDone;
    private int AttackDone;
    private float _tentacleHitDelayTimer = 0;

    [Header("AreaZonePreExplosion (attack2 part1)")]
    public GameObject ExplosionZone;
    [SerializeField] private GameObject _explosionParticles;
    [SerializeField] private float _areaDamageRadius = 1f;
    [SerializeField] private float _areaDamageInterval = 1f;
    [SerializeField] private float _areaDamageAmount = 1f;
    [SerializeField] private float _areaDamageKnockback = 1f;
    [SerializeField] private bool _canAreaDamageZoneDamageMultipleTime = false;
    private List<RB_Health> _alreadyAreaDamageZoneDamaged = new();
    private List<GameObject> _explosion = new List<GameObject>();

    [Header("AreaZoneExplosion (attack2 part2)")]
    public float CooldownBeforeExplosion = 1f;
    [SerializeField] private float _explosionDamage = 1f;
    [SerializeField] private float _explosionKnockback = 1f;
    [SerializeField] private float _explosionRadius = 1f;
    [SerializeField] private bool _canExplosionDamageMultipleTime = false;
    protected float _currentCooldownBeforeTakeDamage, _currentCooldownBeforeExplosion;
    private List<RB_Health> _alreadyExplosionDamaged = new();



    [Header("Clone Attack (attack3)")]
    [SerializeField] private List<GameObject> _enemies;
    [SerializeField] private int _numberOfEnemies;
    [SerializeField] private float _minCooldownForAttack = 30f;
    [SerializeField] private float _maxCooldownForAttack = 45f;
    [SerializeField] private float _scaleHeight = 1f;
    [SerializeField] private float _timeForRescaling = 1f;
    private GameObject _enemy;
    [SerializeField] private List<GameObject> _allEnemies = new List<GameObject>();
    protected float _currentCooldownBeforeReactivate;

    [SerializeField] RB_EnemyAnimation _enemyAnimation;

    protected override void Start()
    {
        base.Start();
        SpawnEnemies();
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
        _currentCooldownBeforeExplosion -= Time.deltaTime;
        _currentCooldownBeforeReactivate -= Time.deltaTime;
        _timeUntilNextState += Time.deltaTime;
        //_mouvement.MoveIntoDirection(PlayerPosition.position);

    }

    private void FixedUpdate()
    {
        switch (CurrentState)
        {
            case BOSSSTATES.Idle:
                
                if (_currentWaitInIdle <= 0) //when waited enough in idle, switch state
                {
                    SpawnEnemies();
                    SwitchBossState();
                }
                break;
            case BOSSSTATES.Moving:
                
                _numberOfAttackDone.Clear();
                _movement.MoveIntoDirection(_currentTarget.position - transform.position,_movementSpeed, 1, _timeForMoving);
                SpawnEnemies();

                if (_timeUntilNextState >= _timeForMoving)
                {
                    SwitchBossState();
                }

                break;
            case BOSSSTATES.Attack1:
                SpawnEnemies();
                if (_numberOfAttackDone.Count < 5)
                {
                    transform.forward = _currentTarget.position - transform.position;
                    if (WaitForAttack1())
                    {
                        TentacleHit();
                        _tentacleHitDelayTimer = _tentacleHitDelay;
                    }
                    CurrentState = BOSSSTATES.Attack1;
                }
                
                else 
                {
                    _timeUntilNextState = 0;
                    CurrentState = BOSSSTATES.Moving;
                }
                
                break;
            case BOSSSTATES.Attack2:
                _alreadyExplosionDamaged.Clear();
                SpawnEnemies();
                _currentWaitInIdle = WaitInIdle;
                CurrentState = BOSSSTATES.Idle; //wait in idle
                break;
            case BOSSSTATES.Attack3:
                SpawnEnemies();
                _currentWaitInIdle = WaitInIdle;
                CurrentState = BOSSSTATES.Idle;
                break;
        }

        if (_currentCooldownBeforeTakeDamage <= 0)
        {
            _alreadyAreaDamageZoneDamaged.Clear();
        }
        EnemyGestion();
    }
    
    private BOSSSTATES SwitchBossState()
    {
        GetTarget();
        _timeUntilNextState = 0;


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
            transform.forward = (_currentTarget.position - transform.position).normalized;
            if (GetTargetDistance() >= 2f) //SWITCH TO ATTACK1
            {
                return CurrentState = BOSSSTATES.Attack1;
            }
        
            if (GetTargetDistance() <= 2f) //SWITCH TO ATTACK2
            {
                ExplosionAttack();
                return CurrentState = BOSSSTATES.Attack2;
            }
            
        }

        if (_currentCooldownAttack3 <= 0 || _allEnemies.Count == 0) //SWITCH TO ATTACK3
        {
            return CurrentState = BOSSSTATES.Attack3;
        }

        else
        {
            return CurrentState = BOSSSTATES.Moving;

            _currentWaitInIdle = WaitInIdle;
            return BOSSSTATES.Idle;
        }
        
        
    }


    public void TentacleHit() //ATTACK 1
    {
        
        _enemyAnimation.TriggerBasicAttack();
        List<RB_Health> alreadyDamaged = new();
        float range = Vector3.Distance(_currentTarget.position, transform.position);
        Vector3 size = new Vector3(_tentacleHitLength, 1, range + 1);
        foreach (Collider enemy in Physics.OverlapBox(transform.position + (transform.forward * _tentacleHitRange / 2), size, transform.rotation))
        {
            if (RB_Tools.TryGetComponentInParent<RB_Health>(enemy.gameObject, out RB_Health enemyHealth))
            {

                if (enemyHealth.Team == Health.Team || alreadyDamaged.Contains(enemyHealth)) continue;

                alreadyDamaged.Add(enemyHealth);
                enemyHealth.TakeDamage(_tentacleHitDamage);
                enemyHealth.TakeKnockback(enemyHealth.transform.position - transform.position, _tentacleHitKnockback);
            }
        }
        if (_tentacleHitParticles)
        {
            Instantiate(_tentacleHitParticles, transform.position + (transform.forward * _tentacleHitRange / 2), transform.rotation);
        }
        AttackDone = new int();
        _numberOfAttackDone.Add(AttackDone);
        _currentCooldownAttack1 = CooldownAttack1;
    }

    private bool WaitForAttack1() //TIMER ATTACK 1
    {
        _tentacleHitDelayTimer -= Time.fixedDeltaTime;
        return (_tentacleHitDelayTimer <= 0);
    }

    public void ExplosionAttack() //ATTACK 2
    {
        //Spawn of the zone attack (attack n°2)

        GameObject Bomb = Instantiate(ExplosionZone, transform.position, Quaternion.identity);
        Bomb.GetComponent<RB_ExplosionZone>().Yog = this;
        Bomb.transform.localScale = new Vector3(_areaDamageRadius * 2, ExplosionZone.transform.localScale.y, _areaDamageRadius * 2);
        _currentCooldownAttack2 = CooldownAttack2;   
    }

    
    public void ApplyExplosionZoneDamage(RB_Health enemyHealth)
    {
        //Application of damages for the zone attack (attack 2)
        if (Health.Team == enemyHealth.Team || (_alreadyAreaDamageZoneDamaged.Contains(enemyHealth) && !_canAreaDamageZoneDamageMultipleTime)) return;
        _alreadyAreaDamageZoneDamaged.Add(enemyHealth);
        enemyHealth.TakeDamage(_areaDamageAmount);
        enemyHealth.TakeKnockback((enemyHealth.transform.position - transform.position).normalized, _areaDamageKnockback);

        //Cooldown
        _currentCooldownBeforeTakeDamage = _areaDamageInterval;
    }

    public void Explosion(RB_Health enemyHealth)
    {
        //Application of damages for the explosion of zone attack (attack 2 part 2)
        if (Health.Team == enemyHealth.Team || (_alreadyExplosionDamaged.Contains(enemyHealth) && !_canExplosionDamageMultipleTime)) return;
        _alreadyExplosionDamaged.Add(enemyHealth);
        enemyHealth.TakeDamage(_explosionDamage);
        enemyHealth.TakeKnockback((enemyHealth.transform.position - transform.position).normalized, _explosionKnockback);
        
        if (_explosionParticles)
            Instantiate(_explosionParticles, transform.position, transform.rotation);
    }

    public void SpawnEnemies()
    {
        List<Vector3> spawnPoints = new List<Vector3>();

        if (_currentCooldownAttack3 <= 0 || _allEnemies.Count == 0)
        {
            for (int i = 0; i < _numberOfEnemies; i++)
            {
                //Set a random spawn inside a sphere around the boss
                Vector3 randomDirection = Random.insideUnitSphere;
                randomDirection.y = 0;
                randomDirection.Normalize();

                float randomDistance = Random.Range(_minMovementDistance, _maxMovementDistance);

                Vector3 spawnPoint = transform.position + randomDirection * randomDistance;

                spawnPoints.Add(spawnPoint);

                //Instantiate random type of enemy present in a list
                int randomIndex = Random.Range(0, _enemies.Count);

                _enemy = Instantiate(_enemies[randomIndex], spawnPoints[i], transform.rotation);
                _enemy.transform.localScale = new Vector3(_scaleHeight, _scaleHeight, _scaleHeight);
                _allEnemies.Add(_enemy);

                //Cooldown
                _currentCooldownAttack3 = Random.Range(_minCooldownForAttack, _maxCooldownForAttack);
            }
        }

    }
    
    public void EnemyGestion()
    {
        //Gestion of enemys (rescaling before spawn)
        foreach (GameObject en in _allEnemies)
        {
            _enemy = en;
            
            if (_enemy.transform.localScale.x <= 1)
            {
                Vector3 rescalingHeight = new Vector3(1 / _timeForRescaling / 60, 1 / _timeForRescaling / 60, 1 / _timeForRescaling / 60);
                _enemy.transform.localScale += rescalingHeight;
                if (_enemy.transform.localScale.x > 1)
                {
                    _enemy.transform.localScale = Vector3.one;
                }
            }

            if (_enemy.GetComponent<RB_Health>().Dead == true)
            {
                _allEnemies.Remove(_enemy);
            }
        }
    } 
}
