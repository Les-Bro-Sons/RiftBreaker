using UnityEngine;
using UnityEngine.Events;

public class RB_Enemy : MonoBehaviour
{
    public UnityEvent EventDead;
    [Header("Spawn")]
    [SerializeField] private bool _isAttachedToAPhase = true; // if false, everything under this in "Spawn" is useless
    [SerializeField] private PHASES _spawnInPhase = PHASES.Infiltration;
    public SpriteRenderer SpriteRenderer; //PLACEHOLDER
    public Animator AiAnimator;

    private Rigidbody _rb;
    private RB_AI_BTTree _btTree;

    private bool _isTombstoned = false;

    private LayerMask _originalExcludeLayer;

    protected Transform _currentTarget;

    protected RB_AiMovement _movement;

    public float ChargeSpecialAttackAmount;
    protected virtual void Awake()
    {
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
    }

    private void SetLayerToTeam()
    {
        int layer;
        TEAMS team = GetComponent<RB_Health>().Team;
        if (team == TEAMS.Ai)
            layer = LayerMask.NameToLayer("Enemy");
        else
            layer = LayerMask.NameToLayer("Ally");

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
            SpriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Ai/Tombstone/Tombstone"); //PLACEHOLDER
            _rb.excludeLayers = ~(1 << LayerMask.NameToLayer("Terrain"));
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
            if (_btTree && !RB_TimeManager.Instance.IsRewinding) _btTree.enabled = true;
            if (AiAnimator && !RB_TimeManager.Instance.IsRewinding) AiAnimator.enabled = true;
            _isTombstoned = false;
            _rb.excludeLayers = _originalExcludeLayer;
        }
    }

    protected float GetTargetDistance() // made so it's easier to modify it for the pawn item (multiple character in the player team)
    {
        return Vector3.Distance(_currentTarget.transform.position, transform.position);
    }

    protected Transform GetTarget() // made so it's easier to modify it for the pawn item (multiple character in the player team)
    {
        return _currentTarget = RB_PlayerController.Instance.transform;
    }
}
