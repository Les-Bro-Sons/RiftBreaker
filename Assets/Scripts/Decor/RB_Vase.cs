using UnityEngine;

public class RB_Vase : MonoBehaviour
{
    //Properties
    [Header("Properties")]
    [SerializeField] private Sprite _brokenSprite;
    [SerializeField] private int _particleAmount;

    //Components
    [Header("Components")]
    [SerializeField] private Collider _vaseCollider;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private RB_Health _health;
    private Transform _transform;

    //Prefabs
    [Header("Prefabs")]
    [SerializeField] private GameObject _vaseParticlesPrefab;

    private void Awake()
    {
        _transform = transform;
        _health = GetComponent<RB_Health>();
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
        for(int i = 0; i < _particleAmount; i++)
            Instantiate(_vaseParticlesPrefab, _transform.position, Quaternion.identity); //Instantiate particles
        _vaseCollider.isTrigger = true; //Desactivate colliders of the vase
        _spriteRenderer.sprite = _brokenSprite; //Set the sprite to the broken one
        _health.TakeDamage(1); //"Kill" the vase
        
    }

    public void UnBreak()
    {
        _vaseCollider.isTrigger = false; //For the rewind, reactive the collision of the vase
    }
}
