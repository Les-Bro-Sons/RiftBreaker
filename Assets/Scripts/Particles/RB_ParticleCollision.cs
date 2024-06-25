using UnityEngine;
using static UnityEngine.ParticleSystem;

public class RB_ParticleCollision : MonoBehaviour
{
    //Components
    private ParticleSystem _ps;

    //Particles
    Particle[] _particles;
    ParticleSystem.ExternalForcesModule _externalForce;

    //Properties
    private float _startTime;
    private bool _isGravitating = false;
    [SerializeField] bool _isLife = false; //If yes, the particles will give you life
    
    void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
        _particles = new Particle[_ps.main.maxParticles]; //Get all particles
        _externalForce = _ps.externalForces; //Get externalForces of all particles
    }

    private void Start()
    {
        _startTime = Time.time;
        _externalForce.enabled = false; //Disable external forces
        Destroy(gameObject, 5);
    }

    private void Update()
    {
        if(Time.time > _startTime + .75f && !_isGravitating)
        {
            GravitToPlayer();
        }
    }

    private void GravitToPlayer()
    {
        //Is attracted by the player
        _isGravitating = true;
        _externalForce.enabled = true;

    }

    void OnParticleCollision(GameObject other)
    {
        int amount = _ps.GetParticles(_particles);
        int currentAmount = _ps.emission.GetBurst(0).maxCount;
        int nearestParticle = 0;
        if(RB_Tools.TryGetComponentInParent<RB_PlayerMovement>(other, out RB_PlayerMovement playerMovement)) //If the collision is the player
        {
            for(int i = 0; i < amount; i++)
            {
                if (Vector3.Distance(_particles[nearestParticle].position, other.transform.position) > Vector3.Distance(_particles[i].position, other.transform.position))
                {
                    nearestParticle = i; //Get the nearest particle of the player
                }
            }
            _particles[nearestParticle].remainingLifetime = 0; //Destroy that particle
            _ps.SetParticles(_particles, amount);
            RB_PlayerAction.Instance.EventOnChargeSpecialAttackGathered?.Invoke();
            if (_isLife)
            {
                playerMovement.gameObject.GetComponent<RB_Health>().Heal(2); //Heal player if particle colides
            }
            else
            {
                if (RB_Tools.TryGetComponentInParent<RB_Enemy>(gameObject, out RB_Enemy enemy))
                {
                    RB_PlayerAction.Instance.AddToSpecialChargeAttack(enemy.ChargeSpecialAttackAmount / (float)currentAmount); //Set the chargeAttackAmount gathered to the charge attack of the player
                }
            }
        }
        

    }
}