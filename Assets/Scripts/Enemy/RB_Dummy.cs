using UnityEngine;

public class RB_Dummy : RB_Enemy
{
    [SerializeField] private string DeathSpawnPrefabName;

    /// <summary>
    /// Handles the death of the dummy enemy.
    /// </summary>
    protected override void Death()
    {
        EventDead?.Invoke();

        // Instantiate the death spawn prefab at a random position near the current position.
        Instantiate(
            Resources.Load<GameObject>("Prefabs/Enemies/" + DeathSpawnPrefabName),
            new Vector3(Random.Range(-3, 3), transform.position.y, Random.Range(-3, 3)),
            transform.rotation
        );

        Tombstone();
    }
}
