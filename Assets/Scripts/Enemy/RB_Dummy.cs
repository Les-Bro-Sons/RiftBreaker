using UnityEngine;

public class RB_Dummy : RB_Enemy
{
    [SerializeField] private GameObject DeathSpawnPrefab;

    protected override void Death()
    {
        Instantiate(DeathSpawnPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
