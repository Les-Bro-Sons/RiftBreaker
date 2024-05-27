using UnityEngine;
using UnityEngine.Events;

public class RB_Enemy : MonoBehaviour
{
    public UnityEvent EventDead;
    [Header("Spawn")]
    [SerializeField] private bool _isAttachedToAPhase = true; // if false, everything under this in "Spawn" is useless
    [SerializeField] private PHASES _spawnInPhase = PHASES.Infiltration;
    public SpriteRenderer SpriteRenderer; //PLACEHOLDER
    private Rigidbody _rb;

    private bool _isTombstoned = false;

    private LayerMask _originalExcludeLayer;

    protected virtual void Awake()
    {
        GetComponent<RB_Health>().EventDeath.AddListener(Death);
        GetComponent<RB_Health>().EventTakeDamage.AddListener(TakeDamage);
        _rb = GetComponent<Rigidbody>();
        _originalExcludeLayer = _rb.excludeLayers;
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
            _isTombstoned = true;
            SpriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Ai/Tombstone/Tombstone"); //PLACEHOLDER
            _rb.excludeLayers = ~(1 << LayerMask.NameToLayer("Terrain"));
            _rb.velocity = Vector3.zero;
        }
    }

    public virtual void UnTombstone() // make the enemy alive again
    {
        if (_isTombstoned)
        {
            _isTombstoned = false;
            _rb.excludeLayers = _originalExcludeLayer;
        }
    }
}
