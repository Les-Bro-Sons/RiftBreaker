using UnityEngine;

public class RB_ScytheZone : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particles;
    private RB_CollisionDetection _collisionDetection;
    private ParticleSystem.ShapeModule _partShape;

    private bool _shouldDealDamage = false;

    //Timer
    float damageTime = 0;

    private void Awake()
    {
        _collisionDetection = GetComponentInChildren<RB_CollisionDetection>();
    }

    private void Start()
    {
        _partShape = _particles.shape;
        _collisionDetection.EventOnEnemyEntered.AddListener(OnEnemyDetected);
    }

    private void Update()
    {
        _partShape.radius = transform.lossyScale.x; //set the radius of particles
        DealDamage();
    }

    private void OnEnemyDetected()
    {
        _shouldDealDamage = true;
    }

    private void DealDamage()
    {
        RB_Scythe currentItem = RB_PlayerAction.Instance.Item as RB_Scythe;
        if (_shouldDealDamage && Time.time > damageTime + currentItem.ZoneDelay)
        {
            damageTime = Time.time;
            foreach(GameObject enemy in _collisionDetection.GetDetectedEnnemies())
            {
                enemy.GetComponent<RB_Health>().TakeDamage(currentItem.ChargedAttackDamage);
            }
        }
    }
}
