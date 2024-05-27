using UnityEngine;

public class RB_Dummy : RB_Enemy
{
    [SerializeField] private string DeathSpawnPrefabName;
    protected override void Death()
    {
        EventDead?.Invoke();
        //Instantiate(DeathSpawnPrefab, transform.position, transform.rotation);
        Instantiate(Resources.Load<GameObject>("Prefabs/Enemies/" + DeathSpawnPrefabName), new Vector3(Random.Range(-3, 3), transform.position.y, Random.Range(-3, 3)), transform.rotation);
        Tombstone();
        //Destroy(gameObject);
    }
}
