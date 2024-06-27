using MANAGERS;
using UnityEngine;
using UnityEngine.AI;

public class RB_Vase : MonoBehaviour
{
    // Properties
    [Header("Properties")]
    [SerializeField] private Sprite _brokenSprite;
    [SerializeField] private int _particleAmount;
    [Range(0, 100)]
    [SerializeField] private int _probLife;

    // Components
    [Header("Components")]
    [SerializeField] private Collider _vaseCollider;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private NavMeshObstacle _navMeshObstacle;
    private RB_Health _health;
    private Transform _transform;
    private Rigidbody _rb;

    // Prefabs
    [Header("Prefabs")]
    [SerializeField] private GameObject _vaseParticlesPrefab;
    [SerializeField] public GameObject _lifeParticlesPrefab;

    private LayerMask _originalExcludeLayer;

    private void Awake()
    {
        _transform = transform;
        _health = GetComponent<RB_Health>();
        _rb = GetComponent<Rigidbody>();
        _originalExcludeLayer = _rb.excludeLayers;
    }

    private void Start()
    {
        _health.EventDeath.AddListener(Break);
        _health.EventHeal.AddListener(UnBreak);
    }

    /// <summary>
    /// Handles collision events with the vase.
    /// If the player is dashing into the vase, it "kills" the vase.
    /// </summary>
    /// <param name="collision">The collision data.</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (!_health.Dead && collision.gameObject.TryGetComponent(out RB_PlayerMovement playerMovement) && playerMovement.IsDashing())
        {
            _health.TakeDamage(_health.HpMax); //"Kill" the vase
        }
    }

    /// <summary>
    /// Handles continuous collision events with the vase.
    /// If the player is dashing into the vase, it "kills" the vase.
    /// </summary>
    /// <param name="collision">The collision data.</param>
    private void OnCollisionStay(Collision collision)
    {
        if (!_health.Dead && collision.gameObject.TryGetComponent(out RB_PlayerMovement playerMovement) && playerMovement.IsDashing())
        {
            _health.TakeDamage(_health.HpMax); //"Kill" the vase
        }
    }

    /// <summary>
    /// Breaks the vase, triggering visual and gameplay changes.
    /// </summary>
    public void Break()
    {
        for (int i = 0; i < _particleAmount; i++)
        {
            Instantiate(_vaseParticlesPrefab, _transform.position, Quaternion.identity); // Instantiate particles
        }
        _rb.excludeLayers = ~(1 << LayerMask.NameToLayer("Terrain") | 1 << LayerMask.NameToLayer("Room")); // Exclude layers from NavMesh carving
        _spriteRenderer.sprite = _brokenSprite; // Set the sprite to the broken one
        int _probTemp = Random.Range(0, 100); // Simulate probability for life particles
        if (100 - _probLife <= _probTemp)
        {
            Instantiate(_lifeParticlesPrefab, _transform.position, Quaternion.identity); // Instantiate life particles based on probability
        }
        _navMeshObstacle.carving = false; // Disable NavMesh carving
        RB_AudioManager.Instance.PlaySFX("BreakingPot", transform.position, false, 0.2f, 1); // Play breaking sound effect
    }

    /// <summary>
    /// Restores the vase to its normal state.
    /// </summary>
    public void UnBreak()
    {
        _rb.excludeLayers = _originalExcludeLayer; // Restore original excluded layers for NavMesh carving
        _navMeshObstacle.carving = true; // Enable NavMesh carving
    }
}
