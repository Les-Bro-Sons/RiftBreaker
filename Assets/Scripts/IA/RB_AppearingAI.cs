using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RB_AppearingAI : MonoBehaviour
{
    private new Transform transform;
    private Rigidbody _rb;
    public SpriteRenderer EntitySpriteRenderer;
    private Material _baseMaterial;

    private Material _dissolveMaterial;

    private ParticleSystem _appearingParticle;

    public float TimeForAppearing = 1.0f;

    private float _appearingTimer = 0;

    public float StartDissolveAmount = 1;
    public float TargetDissolveAmount = 0;

    private void Awake()
    {
        transform = GetComponent<Transform>();
        if (TryGetComponent<RB_Enemy>(out RB_Enemy enemyComponent))
        {
            EntitySpriteRenderer = enemyComponent.SpriteRenderer;
        }
        else if (TryGetComponent<RB_PlayerController>(out RB_PlayerController playerComponent))
        {
            EntitySpriteRenderer = playerComponent.PlayerSpriteRenderer;
        }
        _baseMaterial = new Material(EntitySpriteRenderer.material);
        _dissolveMaterial = new Material(Resources.Load<Material>("Material/DissolveSprite"));
        EntitySpriteRenderer.material = _dissolveMaterial;
        _appearingParticle = Resources.Load<ParticleSystem>("Prefabs/Particles/AppearingParticle");
        _appearingParticle = Instantiate(_appearingParticle, transform.position, Quaternion.identity);
        EnemyGestion();
    }

    private void Start()
    {
        EnemyGestion();
    }

    private void Update()
    {
        EnemyGestion();
        _appearingTimer += Time.deltaTime;
    }

    public void EnemyGestion()
    {

        RB_AI_BTTree enemyTree = null;
        if (TryGetComponent<RB_AI_BTTree>(out enemyTree));

        if (_appearingTimer < TimeForAppearing)
        {
            if (enemyTree != null)
            {
                _rb = GetComponent<Rigidbody>();
                _rb.constraints = RigidbodyConstraints.FreezeAll;
                _rb.detectCollisions = false;
                enemyTree.enabled = false;
            }
            
            _dissolveMaterial.SetFloat("_DissolveAmount", Mathf.Lerp(StartDissolveAmount, TargetDissolveAmount, _appearingTimer / TimeForAppearing));
        }
        else
        {
            if (enemyTree != null)
            {
                _rb.constraints = RigidbodyConstraints.FreezeRotation;
                _rb.detectCollisions = true;
                EntitySpriteRenderer.material = _baseMaterial;
                enemyTree.enabled = true;
            }
            
            Destroy(_appearingParticle.gameObject, 5);
            _appearingParticle.Stop();
            Destroy(this);
        }
    }
}
