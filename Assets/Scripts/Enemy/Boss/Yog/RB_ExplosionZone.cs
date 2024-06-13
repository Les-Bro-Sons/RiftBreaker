using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RB_ExplosionZone : MonoBehaviour
{

    public RB_Yog Yog;
    private RB_CollisionDetection _collisionDetection;
    private Vector3 _baseScale;
    public Vector3 FinalScale;
    private float _beforeExplosionDuration = 5;
    private float _lifetimeTimer = 0;

    private AnimationCurve ExpandCurve;

    private void Awake()
    {
        _collisionDetection = GetComponent<RB_CollisionDetection>();
        _collisionDetection.EventOnEnemyEntered.AddListener(delegate { EnemyEntered(_collisionDetection.GetDetectedObjects()[_collisionDetection.GetDetectedObjects().Count - 1]); });
        _beforeExplosionDuration = Yog.CooldownBeforeExplosion;
        _baseScale = transform.localScale;
        ExpandCurve = Yog.AreaExpandCurve;
    }

    private void Start()
    {
        StartCoroutine(WaitForExplosion());
    }
    private void Update()
    {
        //CheckForEnemies();
        UpdateExplosionZone();
        _lifetimeTimer += Time.deltaTime;
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
        gameObject.transform.localScale = Vector3.Lerp(_baseScale, FinalScale, ExpandCurve.Evaluate(_lifetimeTimer / _beforeExplosionDuration));
    }

    IEnumerator WaitForExplosion()
    {
        yield return new WaitForSeconds(Yog.CooldownBeforeExplosion);
        List<RB_Health> enemyList = new List<RB_Health>();
        foreach (GameObject enemy in _collisionDetection.GetDetectedObjects())
        {
            if (RB_Tools.TryGetComponentInParent<RB_Health>(enemy, out RB_Health enemyHealth))
            {
                enemyList.Add(enemyHealth);
            }
        }
        if (Yog) Yog.Explosion(enemyList);
        Destroy(gameObject);
    }
}
