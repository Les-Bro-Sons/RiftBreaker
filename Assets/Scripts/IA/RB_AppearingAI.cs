using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_AppearingAI : MonoBehaviour
{
    private new Transform transform;
    private Rigidbody _rb;
    private SpriteRenderer _spriteRenderer;
    private Material _baseMaterial;

    private Material _dissolveMaterial;

    public float TimeForAppearing = 1.0f;

    private float _appearingTimer = 0;

    private void Awake()
    {
        transform = GetComponent<Transform>();
        _spriteRenderer = GetComponent<RB_Enemy>().SpriteRenderer;
        _baseMaterial = new Material(_spriteRenderer.material);
        _dissolveMaterial = new Material(Resources.Load<Material>("Material/DissolveSprite"));
        _spriteRenderer.material = _dissolveMaterial;
    }

    private void Update()
    {
        EnemyGestion();
        _appearingTimer += Time.deltaTime;
    }

    public void EnemyGestion()
    {
        RB_AI_BTTree enemyTree = GetComponent<RB_AI_BTTree>();

        if (_appearingTimer < TimeForAppearing)
        {
            _rb = GetComponent<Rigidbody>();
            _rb.constraints = RigidbodyConstraints.FreezeAll;
            _rb.detectCollisions = false;
            enemyTree.enabled = false;
            _dissolveMaterial.SetFloat("_DissolveAmount", Mathf.Lerp(1, 0, _appearingTimer / TimeForAppearing));
        }
        else
        {
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
            _rb.detectCollisions = true;
            _spriteRenderer.material = _baseMaterial;
            enemyTree.enabled = true;
            Destroy(this);
        }
    }
}
