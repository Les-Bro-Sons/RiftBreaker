using System.Collections;
using UnityEngine;

public class RB_ColliderAnim : MonoBehaviour
{

    [SerializeField] Animator _collisionAnimations;

    //Prefab spawner
    private bool _prefabSpawned = false;

    //Components
    Transform _transform;

    private void Start()
    {
        _transform = RB_PlayerAction.Instance.transform;
    }

    public void StopAnimation(string AnimationToStop)
    {
        //Stop the animation
        _collisionAnimations.SetBool(AnimationToStop, false);
    }

    public void SpawnPrefab(string prefabToSpawn)
    {
        print("prefabSpawned");
        if (!_prefabSpawned)
        {
            //Spawn the prefab by his name
            _prefabSpawned = true;
            GameObject newObject = Instantiate(Resources.Load("Prefabs/" + prefabToSpawn), _transform.position, _transform.rotation) as GameObject;
            if (newObject.TryGetComponent<RB_Projectile>(out RB_Projectile projectile))
            {
                newObject.transform.position += _transform.forward * projectile.SpawnDistanceFromPlayer;
                projectile.Team = TEAMS.Player;
            }
            StartCoroutine(ResetSpawnPrefab());
        }
    }

    IEnumerator ResetSpawnPrefab()
    {
        //To prevent from spawning two projectile at once
        yield return new WaitForSeconds(.1f);
        _prefabSpawned = false;
    }
}
