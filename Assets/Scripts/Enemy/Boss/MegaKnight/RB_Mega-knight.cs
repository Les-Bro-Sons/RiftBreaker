using MANAGERS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RB_Mega_knight : RB_Boss
{
    public static RB_Mega_knight Instance;
    public BOSSSTATES CurrentState = BOSSSTATES.Idle;
    public bool _inRoom = false;
    [HideInInspector] public UnityEvent EventPlayMKMusic;

    private float _activationTimer = 0;

    [Header("Slash (attack1)")]
    [SerializeField] private float _slashDamage = 30;
    [SerializeField] private float _slashKnockback = 15;
    [SerializeField] private float _slashRange = 3;
    [SerializeField] private float _slashDelay = 0.5f;
    [SerializeField] private GameObject _slashParticles;
    private float _slashDelayTimer = 0;

    [Header("Spikes (attack2)")]
    public GameObject Spikes;
    [SerializeField] private float _spikesLength = 5;
    [SerializeField] private float _spikesSpaces = 0.75f;
    [SerializeField] private float _spikeDamage = 10;
    [SerializeField] private float _spikeKnockback = 10;
    [SerializeField] private bool _canSpikeDamageMultipleTime = false;
    [SerializeField] private float _spikeDelayIncrementation = 0.1f;
    private List<RB_Health> _alreadySpikeDamaged = new();

    [Header("Jump (attack3)")]
    public float JumpHeight = 5f;
    public float DamageRadius = 5f;
    [SerializeField] private AnimationCurve _jumpAttackCurve;
    [SerializeField] private float _jumpDuration = 1;
    [SerializeField] private float _landingRadius = 2;
    [SerializeField] private float _landingDamage = 20;
    [SerializeField] private float _landingKnockback = 10;
    [SerializeField] private GameObject _landingParticles;
    private float _currentJumpDuration = 0;
    private Vector3 _jumpStartPos;
    private Vector3 _jumpEndPos;

    //Animation
    [SerializeField] RB_EnemyAnimation _enemyAnimation;

    protected override void Awake(){
        transform = GetComponent<Transform>();
        if (Instance == null) {
            Instance = this;
        }
        else { 
            DestroyImmediate(gameObject);
        }
        base.Awake();
    }

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
    }

    private void FixedUpdate()
    {
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

        switch (CurrentState)
        {
            case BOSSSTATES.Idle:
                if (_currentWaitInIdle <= 0) //when waited enough in idle, switch state
                {
                    SwitchBossState();
                }
                break;
            case BOSSSTATES.Moving:
                _movement.MoveToPosition(_currentTarget.position); //move to the target
                SwitchBossState();
                break;
            case BOSSSTATES.Attack1:
                if (WaitForSlash())
                {
                    Slash();
                    SwitchBossState();
                }
                break;
            case BOSSSTATES.Attack2:
                _currentWaitInIdle = WaitInIdle;
                CurrentState = BOSSSTATES.Idle; //wait in idle
                break;
            case BOSSSTATES.Attack3:
                JumpAttack();
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
                _alreadySpikeDamaged.Clear();
                KickAttack();
                return CurrentState = BOSSSTATES.Attack2;
            }

            if (_currentCooldownAttack3 <= 0 && GetTargetDistance() > 7f) //SWITCH TO ATTACK3
            {
                StartJumpAttack();
                return CurrentState = BOSSSTATES.Attack3;
            }
        }
        
        return CurrentState = BOSSSTATES.Moving;
    }

    private void Room()
    {

        if (_inRoom == false)
        {
            EventPlayMKMusic.Invoke();
            _inRoom = true;
        }
        if (_inRoom == true)
        {
            return;
        }
    }
    private bool WaitForSlash() //TIMER ATTACK 1
    {
        _slashDelayTimer -= Time.fixedDeltaTime;
        return (_slashDelayTimer <= 0);
    }

    public void Slash() //ATTACK 1
    {
        //Animations
        _enemyAnimation.TriggerBasicAttack();
        RB_AudioManager.Instance.PlaySFX("BigSwooosh", transform.position, false, 1, 1f);
        List<RB_Health> alreadyDamaged = new();
        foreach (Collider enemy in Physics.OverlapBox(transform.position + (transform.forward * _slashRange / 2), Vector3.one * (_slashRange / 2f), transform.rotation))
        {
            if (RB_Tools.TryGetComponentInParent<RB_Health>(enemy.gameObject, out RB_Health enemyHealth))
            {
                
                if (enemyHealth.Team == Health.Team || alreadyDamaged.Contains(enemyHealth)) continue;

                alreadyDamaged.Add(enemyHealth);
                enemyHealth.TakeDamage(_slashDamage);
                enemyHealth.TakeKnockback(RB_Tools.GetHorizontalDirection(enemyHealth.transform.position, transform.position) ,_slashKnockback);
            }
        }
        if (_slashParticles)
        {
            Instantiate(_slashParticles, transform.position + (transform.forward * _slashRange / 2), transform.rotation);
        }
        _currentCooldownAttack1 = CooldownAttack1;
    }

    private void StartJumpAttack() //START ATTACK 3
    {
        //Animations
        _enemyAnimation.TriggerThirdAttack();

        _currentJumpDuration = 0;
        _jumpStartPos = transform.position;
        _jumpEndPos = _currentTarget.position;
        BossRB.velocity = Vector3.zero;
    }

    public void JumpAttack() //ATTACK 3
    {
        //jump calculation
        _currentJumpDuration += Time.fixedDeltaTime;
        float percentComplete = _currentJumpDuration / _jumpDuration;
        float yPos = _jumpAttackCurve.Evaluate(percentComplete) * JumpHeight;
        Vector3 horizontalPos = Vector3.Lerp(_jumpStartPos, _jumpEndPos, percentComplete);
        BossRB.MovePosition(new Vector3(horizontalPos.x, yPos, horizontalPos.z));
        
        if (_currentJumpDuration >= _jumpDuration) //when landed
        {
            List<RB_Health> alreadyDamaged = new();
            foreach (Collider collider in Physics.OverlapSphere(transform.position, _landingRadius)) //landing collider check
            {
                if (RB_Tools.TryGetComponentInParent<RB_Health>(collider.gameObject, out RB_Health enemyHealth))
                {
                    if (enemyHealth.Team == Health.Team || alreadyDamaged.Contains(enemyHealth)) continue;
                    alreadyDamaged.Add(enemyHealth);
                    enemyHealth.TakeDamage(_landingDamage);
                    enemyHealth.TakeKnockback(RB_Tools.GetHorizontalDirection(collider.transform.position, transform.position), _landingKnockback);
                }
            }

            if (_landingParticles)
            {
                Instantiate(_landingParticles, transform.position, transform.rotation);
            }

            //ENDING STATE ATTACK 3
            RB_AudioManager.Instance.PlaySFX("Jump_Attack_Viking_Horn", transform.position, false, 0, 1f);
            _currentCooldownAttack3 = CooldownAttack3;
            SwitchBossState();
        }
    }

    public void KickAttack() //ATTACK 2
    {
        /*
        float startX = transform.position.x - _rangeOfAttack.x / 2;
        float startY = transform.position.y - _rangeOfAttack.y / 2;
        float spacingX = _rangeOfAttack.x / RowsOfSpikes;
        float spacingY = _rangeOfAttack.y / ColumnsOfSpikes;

        for (int i = 0; i < RowsOfSpikes; i++)
        {
            for (int j = 0; j < ColumnsOfSpikes; j++)
            {
                Vector3 SpawnPosition = new Vector3(startX + j * spacingX, startY + i * spacingY, 0);
                Instantiate(Spikes, SpawnPosition, Quaternion.identity, transform);
            }
        }
        */

        //Animations
        

        float currentLength = 0;
        Vector3 placingdir = (_currentTarget.position - transform.position);
        placingdir = new Vector3(placingdir.x, 0, placingdir.z).normalized;
        Vector3 placingPos = transform.position + (placingdir * _spikesSpaces);
        placingPos.y = Spikes.transform.position.y;

        BossRB.MoveRotation(Quaternion.LookRotation(placingdir));
        _enemyAnimation.TriggerSecondAttack();

        float delay = 0;

        while (currentLength < _spikesLength)
        {
            placingPos.y = Spikes.transform.position.y;
            RB_Spikes spike = Instantiate(Spikes, placingPos, Quaternion.identity).GetComponent<RB_Spikes>();
            spike.MegaKnight = this;
            spike.GoingUpDelay = delay;

            delay += _spikeDelayIncrementation;
            placingPos += placingdir * _spikesSpaces;
            currentLength += _spikesSpaces;
        }

        _currentCooldownAttack2 = CooldownAttack2;
    }

    public void ApplySpikeDamage(RB_Health enemyHealth)
    {
        if (Health.Team == enemyHealth.Team || (_alreadySpikeDamaged.Contains(enemyHealth) && !_canSpikeDamageMultipleTime)) return;

        _alreadySpikeDamaged.Add(enemyHealth);
        enemyHealth.TakeDamage(_spikeDamage);
        enemyHealth.TakeKnockback(RB_Tools.GetHorizontalDirection(enemyHealth.transform.position, transform.position), _spikeKnockback);
    }
    

    float CalculateVerticalJumpSpeed(float distance)
    {
        float verticalSpeed = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * JumpHeight);
        return verticalSpeed;
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            //_isGrounded = true;
            //DealDamageToPlayer();
        }
    }
}
