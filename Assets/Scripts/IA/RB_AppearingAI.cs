using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_AppearingAI : MonoBehaviour
{
    private new Transform transform;
    private Rigidbody _rb;

    private float _startScale;
    public float TimeForRescaling = 1.0f;

    private float _rescaleTimer = 0;

    private void Awake()
    {
        transform = GetComponent<Transform>();
        _startScale = transform.localScale.x;
    }

    private void Update()
    {
        EnemyGestion();
        _rescaleTimer += Time.deltaTime;
    }

    public void EnemyGestion()
    {
        RB_AI_BTTree enemyTree = GetComponent<RB_AI_BTTree>();

        if (_rescaleTimer < TimeForRescaling)
        {
            _rb = GetComponent<Rigidbody>();
            _rb.constraints = RigidbodyConstraints.FreezeAll;
            _rb.detectCollisions = false;
            enemyTree.enabled = false;


            Vector3 rescalingHeight = Vector3.Lerp(Vector3.one * _startScale, Vector3.one, _rescaleTimer / TimeForRescaling);
            transform.localScale = rescalingHeight;
        }
        else
        {
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
            _rb.detectCollisions = true;
            transform.localScale = Vector3.one;
            enemyTree.enabled = true;
            Destroy(this);
        }
    }
}
