using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RB_RainZone : MonoBehaviour
{

    public RB_RobertLenec Sylvashot;
    private RB_CollisionDetection _collisionDetection;
    

    [Header("Main Properties")]
    [SerializeField] private float _lifetime = 1;



    private void Awake()
    {
        _collisionDetection = GetComponent<RB_CollisionDetection>();
        _collisionDetection.EventOnEnemyEntered.AddListener(delegate { EnemyEntered(_collisionDetection.GetDetectedObjects()[_collisionDetection.GetDetectedObjects().Count - 1]); });
        Destroy(gameObject, _lifetime);
    }

    private void Update()
    {
        CheckForEnemies();
    }

    private void CheckForEnemies()
    {
        foreach (GameObject enemy in _collisionDetection.GetDetectedObjects())
        {
            if (RB_Tools.TryGetComponentInParent<RB_Health>(enemy, out RB_Health enemyHealth))
            {
                if (Sylvashot)
                {
                    Sylvashot.ApplyRainZoneDamage(enemyHealth);
                }
            }
        }
    }

    private void EnemyEntered(GameObject enemy)
    {
        if (RB_Tools.TryGetComponentInParent<RB_Health>(enemy, out RB_Health enemyHealth))
        {
            if (Sylvashot)
            {
                Sylvashot.ApplyRainZoneDamage(enemyHealth);
            }
        }
    }


}
