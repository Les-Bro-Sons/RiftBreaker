using System.Collections;
using UnityEngine;

public class RB_EnemyAnimation : MonoBehaviour
{
    // Components
    private Animator _enemyAnimator;
    private Rigidbody _rb;
    private Transform _transform;

    // Prefab spawner state
    private bool _prefabSpawned = false;

    /// <summary>
    /// Initializes components on Awake.
    /// </summary>
    private void Awake()
    {
        _enemyAnimator = GetComponent<Animator>();
    }

    /// <summary>
    /// Initializes references to the Rigidbody and Transform of the parent RB_Enemy on Start.
    /// </summary>
    private void Start()
    {
        if (RB_Tools.TryGetComponentInParent<RB_Enemy>(gameObject, out RB_Enemy enemy))
        {
            _rb = enemy.GetComponent<Rigidbody>();
            _transform = _rb.transform;
        }
    }

    /// <summary>
    /// Updates the animation parameters based on the enemy's movement.
    /// </summary>
    private void UpdateAnim()
    {
        _enemyAnimator.SetFloat("Horizontal", _transform.forward.normalized.x);
        _enemyAnimator.SetFloat("Vertical", _transform.forward.normalized.z);
        _enemyAnimator.SetFloat("Speed", _rb.velocity.magnitude);
        Debug.DrawRay(_transform.position, _transform.forward);
    }

    /// <summary>
    /// Spawns a prefab with the given name at the enemy's position.
    /// </summary>
    /// <param name="prefabToSpawn">The name of the prefab to spawn.</param>
    public void SpawnPrefab(string prefabToSpawn)
    {
        if (!_prefabSpawned)
        {
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

    /// <summary>
    /// Coroutine to reset the prefab spawned state after a short delay.
    /// To prevent from spawning two projectile at once
    /// </summary>
    /// <returns>IEnumerator for the coroutine.</returns>
    private IEnumerator ResetSpawnPrefab()
    {
        yield return new WaitForSeconds(0.1f);
        _prefabSpawned = false;
    }

    /// <summary>
    /// Updates the animation every frame.
    /// </summary>
    private void Update()
    {
        UpdateAnim();
    }

    /// <summary>
    /// Triggers the basic attack animation.
    /// </summary>
    public void TriggerBasicAttack()
    {
        _enemyAnimator.SetTrigger("BasicAttack");
    }

    /// <summary>
    /// Triggers the second attack animation.
    /// </summary>
    public void TriggerSecondAttack()
    {
        _enemyAnimator.SetTrigger("SecondAttack");
    }

    /// <summary>
    /// Triggers the third attack animation.
    /// </summary>
    public void TriggerThirdAttack()
    {
        _enemyAnimator.SetTrigger("ThirdAttack");
    }
}
