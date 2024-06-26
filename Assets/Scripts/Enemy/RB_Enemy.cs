using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RB_Enemy : MonoBehaviour
{
    protected new Transform transform;

    public UnityEvent EventDead;
    [Header("Spawn")]
    [SerializeField] private bool _isAttachedToAPhase = true; // if false, everything under this in "Spawn" is useless
    [SerializeField] private PHASES _spawnInPhase = PHASES.Infiltration;
    public SpriteRenderer SpriteRenderer; //PLACEHOLDER
    public Animator AiAnimator;

    protected Rigidbody _rb;
    protected RB_AI_BTTree _btTree;

    private bool _isTombstoned = false;

    private LayerMask _originalExcludeLayer;

    protected Transform _currentTarget;

    protected RB_AiMovement _movement;

    public float ChargeSpecialAttackAmount;

    private Sprite _spriteBeforeDeath;

    [HideInInspector] public UnityEvent EventAllyTeam;
    [HideInInspector] public UnityEvent EventEnemyTeam;

    protected virtual void Awake()
    {
        transform = GetComponent<Transform>();
        GetComponent<RB_Health>().EventDeath.AddListener(Death);
        GetComponent<RB_Health>().EventTakeDamage.AddListener(TakeDamage);
        _rb = GetComponent<Rigidbody>();
        _originalExcludeLayer = _rb.excludeLayers;
        _movement = GetComponent<RB_AiMovement>();
        _btTree = GetComponent<RB_AI_BTTree>();
        SetLayerToTeam();
    }

    protected virtual void Start()
    {
        if (_isAttachedToAPhase && _spawnInPhase != RB_LevelManager.Instance.CurrentPhase)
        {
            RB_LevelManager.Instance.SaveEnemyToPhase(_spawnInPhase, gameObject);
            return;
        }
        Spawned();
        GetTarget();
        SetLayerToTeam();
    }

    private void SetLayerToTeam()
    {
        int layer;
        TEAMS team = GetComponent<RB_Health>().Team;
        if (team == TEAMS.Ai)
        {
            layer = LayerMask.NameToLayer("Enemy");
            EventEnemyTeam?.Invoke();
        }
        else
        {
            layer = LayerMask.NameToLayer("Ally");
            EventAllyTeam?.Invoke();
        }

        gameObject.layer = layer;
        SetLayerToAllChildren(layer, transform);
    }

    private void SetLayerToAllChildren(int layer, Transform obj)
    {
        foreach(Transform child in obj)
        {
            child.gameObject.layer = layer;
            SetLayerToAllChildren(layer, child);
        }
    }  


    public virtual void Spawned() //when the enemy is spawned
    {

    }

     protected virtual void TakeDamage()
     {
    
     }

    protected virtual void Death()
    {
        EventDead?.Invoke();
        Tombstone();
        //Destroy(gameObject);
    }

    public virtual void Tombstone() // make the enemy a tombstone (dead)
    {
        if (!_isTombstoned)
        {
            if (_btTree) _btTree.enabled = false;
            if (AiAnimator) AiAnimator.enabled = false;
            _isTombstoned = true;
            _spriteBeforeDeath = SpriteRenderer.sprite;
            SpriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Ai/Tombstone/Tombstone"); //PLACEHOLDER
            _rb.excludeLayers = ~(1 << LayerMask.NameToLayer("Terrain") | 1 << LayerMask.NameToLayer("Room"));
            _rb.velocity = Vector3.zero;
            //Create charge special attack particles
            GameObject instantiatedChargeSpecialAttackParticleSystem = Instantiate(RB_LevelManager.Instance.ChargeSpecialAttackParticlePrefab, transform);
            instantiatedChargeSpecialAttackParticleSystem.transform.position = transform.position;
        }
    }

    public virtual void UnTombstone() // make the enemy alive again
    {
        if (_isTombstoned)
        {
            if (_btTree && !RB_TimeManager.Instance.IsRewinding)
            {
                _btTree.Root.SetData("target", null);
                _btTree.enabled = true;
                _btTree.BoolDictionnary.Clear();
            }
            SpriteRenderer.sprite = _spriteBeforeDeath;
            if (AiAnimator && !RB_TimeManager.Instance.IsRewinding) AiAnimator.enabled = true;
            _isTombstoned = false;
            _rb.excludeLayers = _originalExcludeLayer;
            SetLayerToTeam();
        }
    }

    protected float GetTargetDistance() // made so it's easier to modify it for the pawn item (multiple character in the player team)
    {
        if (_currentTarget == null) GetTarget();
        return Vector3.Distance(_currentTarget.transform.position, transform.position);
    }

    protected Transform GetTarget(TARGETMODE targetMode = TARGETMODE.Closest) // made so it's easier to modify it for the pawn item (multiple character in the player team)
    {
        List<RB_Health> _enemies = new();
        RB_Health bossHealth = GetComponent<RB_Health>();

        int? room = RB_RoomManager.Instance.GetEntityRoom(bossHealth.Team, gameObject);
        if (room == null) return _currentTarget = RB_PlayerController.Instance.transform;

        _enemies = (bossHealth.Team == TEAMS.Ai)
            ? RB_RoomManager.Instance.GetDetectedAllies(room.Value).ToList()
            : RB_RoomManager.Instance.GetDetectedEnemies(room.Value).ToList();

        float nearbyDetectionRange = 5;
        if (_enemies.Count == 0)
        {
            int allyLayer = (bossHealth.Team == TEAMS.Ai) ? 6 : 9;
            foreach (Collider collider in Physics.OverlapSphere(transform.position, nearbyDetectionRange, ~((1 << 3) | (1 << allyLayer) | (1 << 10))))
            {
                if (RB_Tools.TryGetComponentInParent<RB_Health>(collider.gameObject, out RB_Health enemyHealth)
                    && !enemyHealth.Dead
                    && enemyHealth.Team != bossHealth.Team
                    && Physics.Raycast(transform.position, (enemyHealth.transform.position - transform.position).normalized, out RaycastHit hit, nearbyDetectionRange, ~((1 << allyLayer) | (1 << 10)))
                    && RB_Tools.TryGetComponentInParent<RB_Health>(hit.collider.gameObject, out RB_Health enemyCheck)
                    && enemyCheck == enemyHealth
                    && !_enemies.Contains(enemyHealth))
                {
                    _enemies.Add(enemyHealth);
                }
            }
        }

        _enemies.RemoveAll(enemy => enemy.Dead);

        if (_enemies.Count == 0)
        {
            if (_currentTarget != null)
            {
                Transform target = _currentTarget;
                if (RB_Tools.TryGetComponentInParent<RB_Health>(target, out RB_Health currentTarget) && !currentTarget.Dead && Vector3.Distance(transform.position, target.position) < nearbyDetectionRange / 1.5f)
                {
                    return _currentTarget;
                }
            }

            return _currentTarget = RB_PlayerController.Instance.transform;
        }

        RB_Health targetEnemy = null;
        float targetDistance = Mathf.Infinity;

        switch (targetMode)
        {
            case TARGETMODE.Closest:
                foreach (RB_Health enemy in _enemies)
                {
                    float enemyDistance = Vector3.Distance(transform.position, enemy.transform.position);
                    if (enemyDistance < targetDistance)
                    {
                        targetDistance = enemyDistance;
                        targetEnemy = enemy;
                    }
                }
                break;
            case TARGETMODE.Furthest:
                targetDistance = 0;
                foreach (RB_Health enemy in _enemies)
                {
                    targetDistance = 0;
                    float enemyDistance = Vector3.Distance(transform.position, enemy.transform.position);
                    if (enemyDistance > targetDistance)
                    {
                        targetDistance = enemyDistance;
                        targetEnemy = enemy;
                    }
                }
                break;
            case TARGETMODE.Random:
                targetEnemy = _enemies[Random.Range(0, _enemies.Count)];
                break;
        }

        return _currentTarget = targetEnemy.transform;
    }
}
