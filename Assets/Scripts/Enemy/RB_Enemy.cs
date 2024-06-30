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
    public SpriteRenderer SpriteRenderer; // Placeholder
    public Animator AiAnimator;

    protected Rigidbody _rb;
    protected RB_AI_BTTree _btTree;

    private bool _isTombstoned = false;

    private LayerMask _originalExcludeLayer;

    protected Transform _currentTarget;

    protected RB_AiMovement _movement;

    public float ChargeSpecialAttackAmount;

    private Sprite _spriteBeforeDeath;

    [HideInInspector] public UnityEvent EventJoinAllyTeam;
    [HideInInspector] public UnityEvent EventJoinEnemyTeam;

    /// <summary>
    /// Initialization of the enemy. Sets up references and initial states.
    /// </summary>
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

    /// <summary>
    /// Called on the frame when the script is enabled. Handles enemy spawning logic.
    /// </summary>
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

    /// <summary>
    /// Sets the layer of the enemy and all its children based on its team.
    /// </summary>
    public void SetLayerToTeam()
    {
        int layer;
        TEAMS team = GetComponent<RB_Health>().Team;
        if (team == TEAMS.Ai)
        {
            layer = LayerMask.NameToLayer("Enemy");
            EventJoinEnemyTeam?.Invoke();
        }
        else
        {
            layer = LayerMask.NameToLayer("Ally");
            EventJoinAllyTeam?.Invoke();
        }

        gameObject.layer = layer;
        SetLayerToAllChildren(layer, transform);
    }

    /// <summary>
    /// Recursively sets the layer of all children of a given transform.
    /// </summary>
    private void SetLayerToAllChildren(int layer, Transform obj)
    {
        foreach (Transform child in obj)
        {
            child.gameObject.layer = layer;
            SetLayerToAllChildren(layer, child);
        }
    }

    /// <summary>
    /// Called when the enemy is spawned.
    /// </summary>
    public virtual void Spawned()
    {
        // Custom spawn logic can be implemented in derived classes.
    }

    /// <summary>
    /// Called when the enemy takes damage.
    /// </summary>
    protected virtual void TakeDamage()
    {
        // Custom damage handling can be implemented in derived classes.
    }

    /// <summary>
    /// Called when the enemy dies.
    /// </summary>
    protected virtual void Death()
    {
        EventDead?.Invoke();
        Tombstone();
    }

    /// <summary>
    /// Turns the enemy into a tombstone.
    /// </summary>
    public virtual void Tombstone()
    {
        if (!_isTombstoned)
        {
            if (_btTree) _btTree.enabled = false;
            if (AiAnimator) AiAnimator.enabled = false;
            _isTombstoned = true;
            _spriteBeforeDeath = SpriteRenderer.sprite;
            SpriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Ai/Tombstone/Tombstone"); // Placeholder
            _rb.excludeLayers = ~(1 << LayerMask.NameToLayer("Terrain") | 1 << LayerMask.NameToLayer("Room"));
            _rb.velocity = Vector3.zero;
            GameObject instantiatedChargeSpecialAttackParticleSystem = Instantiate(RB_LevelManager.Instance.ChargeSpecialAttackParticlePrefab, transform);
            instantiatedChargeSpecialAttackParticleSystem.transform.position = transform.position;
        }
    }

    /// <summary>
    /// Revives the enemy from a tombstone state.
    /// </summary>
    public virtual void UnTombstone()
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

    /// <summary>
    /// Gets the distance to the current target.
    /// </summary>
    /// <returns>Distance to the current target.</returns>
    protected float GetTargetDistance()
    {
        if (_currentTarget == null) GetTarget();
        return Vector3.Distance(_currentTarget.transform.position, transform.position);
    }

    /// <summary>
    /// Gets a target based on the specified target mode.
    /// </summary>
    /// <param name="targetMode">Mode for selecting the target.</param>
    /// <returns>Transform of the selected target.</returns>
    protected Transform GetTarget(TARGETMODE targetMode = TARGETMODE.Closest)
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
