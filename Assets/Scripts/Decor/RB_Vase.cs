using MANAGERS;
using UnityEngine;
using UnityEngine.AI;

public class RB_Vase : MonoBehaviour
{
    //Properties
    [Header("Properties")]
    [SerializeField] private Sprite _brokenSprite;
    [SerializeField] private int _particleAmount;
    [Range(0, 100)][SerializeField] private int _probLife;

    //Components
    [Header("Components")]
    [SerializeField] private Collider _vaseCollider;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private NavMeshObstacle _navMeshObstacle;
    private RB_Health _health;
    private Transform _transform;
    private Rigidbody _rb;

    //Prefabs
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


    private void OnCollisionEnter(Collision collision)
    {
        if (!_health.Dead && collision.gameObject.TryGetComponent(out RB_PlayerMovement playerMovement) && playerMovement.IsDashing())
        {
            Break(); //When the player dash into the vase, break
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!_health.Dead && collision.gameObject.TryGetComponent(out RB_PlayerMovement playerMovement) && playerMovement.IsDashing())
        {
            Break(); //When the player dash into the vase, break
        }
    }

    public void Break()
    {
        for (int i = 0; i < _particleAmount; i++)
            Instantiate(_vaseParticlesPrefab, _transform.position, Quaternion.identity); //Instantiate particles
        _rb.excludeLayers = ~(1 << LayerMask.NameToLayer("Terrain") | 1 << LayerMask.NameToLayer("Room"));
        //_vaseCollider.isTrigger = true; //Desactivate colliders of the vase
        _spriteRenderer.sprite = _brokenSprite; //Set the sprite to the broken one
        _health.TakeDamage(1); //"Kill" the vase
        int _probTemp = Random.Range(0, 100); //take a number between 0 to 100 to simulate a %
        if (100 - _probLife <= _probTemp) //100 - _problife = _problife%, basically if _problife is under or equal = probtemp then
        {
            Instantiate(_lifeParticlesPrefab, _transform.position, Quaternion.identity); //Instantiate particles of life
        }
        _navMeshObstacle.carving = false;
        RB_AudioManager.Instance.PlaySFX("BreakingPot", transform.position, false, 0.2f, 0.6f);

    }

    public void UnBreak()
    {
        _rb.excludeLayers = _originalExcludeLayer;
        //_vaseCollider.isTrigger = false; //For the rewind, reactive the collision of the vase
        _navMeshObstacle.carving = true;
    }
}
