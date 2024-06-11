using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RB_ExplosionZone : MonoBehaviour
{

    public RB_Yog Yog;
    private RB_CollisionDetection _collisionDetection;
    private Vector3 _scaleChange;

    private void Awake()
    {
        _collisionDetection = GetComponent<RB_CollisionDetection>();
        _collisionDetection.EventOnEnemyEntered.AddListener(delegate { EnemyEntered(_collisionDetection.GetDetectedObjects()[_collisionDetection.GetDetectedObjects().Count - 1]); });
        Destroy(gameObject, Yog.CooldownBeforeExplosion);
    }

    private void Start()
    {
        StartCoroutine(WaitForExplosion());
    }
    private void Update()
    {
        CheckForEnemies();
        UpdateExplosionZone();
    }

    private void CheckForEnemies()
    {
        foreach (GameObject enemy in _collisionDetection.GetDetectedObjects())
        {
            if (RB_Tools.TryGetComponentInParent<RB_Health>(enemy, out RB_Health enemyHealth))
            {
                if (Yog)
                {
                    Yog.ApplyExplosionZoneDamage(enemyHealth);
                }
            }
        }
    }

    private void EnemyEntered(GameObject enemy)
    {
        if (RB_Tools.TryGetComponentInParent<RB_Health>(enemy, out RB_Health enemyHealth))
        {
            if (Yog)
            {
                Yog.ApplyExplosionZoneDamage(enemyHealth);
            }
        }
    }

    public void UpdateExplosionZone()
    {
        _scaleChange = new Vector3(0.1f, 0.1f, 0.1f);
        gameObject.transform.localScale += _scaleChange;
    }

    IEnumerator WaitForExplosion()
    {
        yield return new WaitForSeconds(Yog.CooldownBeforeExplosion - 0.1f);
        foreach (GameObject enemy in _collisionDetection.GetDetectedObjects())
        {
            if (RB_Tools.TryGetComponentInParent<RB_Health>(enemy, out RB_Health enemyHealth))
            {
                if (Yog)
                    Yog.Explosion(enemyHealth);
            }
        }
    }
}
