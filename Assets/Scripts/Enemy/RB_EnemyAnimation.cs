using System.Collections;
using UnityEngine;

public class RB_EnemyAnimation : MonoBehaviour
{
    //Components
    Animator _enemyAnimator;
    Rigidbody _rb;
    Transform _transform;
    

    //Prefab spawner
    private bool _prefabSpawned = false;

    private void Awake()
    {
        _enemyAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        if(RB_Tools.TryGetComponentInParent<RB_Enemy>(gameObject, out RB_Enemy enemy))
        {
            _rb = enemy.GetComponent<Rigidbody>();
            _transform = _rb.transform;
        }
            
    }

    private void UpdateAnim()
    {
        _enemyAnimator.SetFloat("Horizontal", _transform.forward.normalized.x);
        _enemyAnimator.SetFloat("Vertical", _transform.forward.normalized.z);
        _enemyAnimator.SetFloat("Speed", _rb.velocity.magnitude);
        Debug.DrawRay(_transform.position, _transform.forward);
    }

    public void SpawnPrefab(string prefabToSpawn)
    {
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

    private void Update()
    {
        UpdateAnim();
    }

    public void TriggerBasicAttack()
    {
        _enemyAnimator.SetTrigger("BasicAttack");
    }

    public void TriggerSecondAttack()
    {
        _enemyAnimator.SetTrigger("SecondAttack");
    }

    public void TriggerThirdAttack()
    {
        _enemyAnimator.SetTrigger("ThirdAttack");
    }
}
