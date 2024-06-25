using MANAGERS;
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
    public float AreaExpandingTime = 5;
    private float _lifetimeTimer = 0;

    public AnimationCurve ExpandCurve;

    private void Awake()
    {
        _collisionDetection = GetComponent<RB_CollisionDetection>();
        _collisionDetection.EventOnEnemyEntered.AddListener(delegate { EnemyEntered(_collisionDetection.GetDetectedEnnemies()[_collisionDetection.GetDetectedEnnemies().Count - 1]); });
        _baseScale = transform.localScale;
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
        foreach (GameObject enemy in _collisionDetection.GetDetectedEnnemies())
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
        gameObject.transform.localScale = Vector3.Lerp(_baseScale, FinalScale, ExpandCurve.Evaluate(_lifetimeTimer / AreaExpandingTime));
    }

    IEnumerator WaitForExplosion()
    {
        yield return new WaitForSeconds(AreaExpandingTime);
        List<RB_Health> enemyList = new List<RB_Health>();
        RB_AudioManager.Instance.PlaySFX("Explosion_Sound", transform.position, false, 0f, 1f);
        foreach (GameObject enemy in _collisionDetection.GetDetectedEnnemies())
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
