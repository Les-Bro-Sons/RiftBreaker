using System.Collections;
using UnityEngine;

public class RB_PlayerAnim : MonoBehaviour
{
    //Components
    Animator _playerAnimator;
    [SerializeField] RB_PlayerMovement _playerMovement;
    private Transform _playerTransform;
    private RB_PlayerAction _playerAction;

    //conditions
    private bool _directionGot = false;
    private bool _prefabSpawned = false;

    //Attack
    Vector3 directionToAttack = new();


    private void Awake()
    {
        _playerAnimator = GetComponent<Animator>();
        if (RB_Tools.TryGetComponentInParent<RB_PlayerAction>(gameObject, out RB_PlayerAction playerAction))
        {
            _playerAction = playerAction;
        }
        _playerTransform = _playerAction.transform;
    }

    private void UpdateAnimation()
    {
        if (!_playerAction.IsDoingAnyAttack())
        {
            //Constantly update the direction of the player to the animators if he's not attacking
            _playerAnimator.SetFloat("Horizontal", _playerTransform.forward.x);
            _playerAnimator.SetFloat("Vertical", _playerTransform.forward.z);
            _directionGot = false;
        }
        else
        {
            if (RB_InputManager.Instance.IsMouse)
            {
                //If player is attacking get the position of the mouse and shoot it towards it
                if (!_directionGot && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
                {
                    directionToAttack = new Vector3((hit.point - _playerTransform.position).x, 0, (hit.point - _playerTransform.position).z);
                    _directionGot = true;
                }
                _playerTransform.forward = directionToAttack;
                _playerAnimator.SetFloat("Horizontal", directionToAttack.x);
                _playerAnimator.SetFloat("Vertical", directionToAttack.z);
            }
        }
        //Constantly set the speed of the player to the player animator
        _playerAnimator.SetFloat("Speed", _playerMovement.GetVelocity().magnitude);

    }

    public void StopPlayerAnimation(string AnimationToStop)
    {
        //Stop the animation wanted
        _playerAnimator.SetBool(AnimationToStop, false);
    }

    public void SpawnPrefab(string prefabToSpawn)
    {
        if (!_prefabSpawned)
        {
            //Spawn the prefab by his name
            _prefabSpawned = true;
            GameObject newObject = Instantiate(Resources.Load("Prefabs/Projectiles/" + prefabToSpawn), _playerTransform.position, _playerTransform.rotation) as GameObject;
            if (newObject.TryGetComponent<RB_Projectile>(out RB_Projectile projectile))
            {
                newObject.transform.position += _playerTransform.forward * projectile.SpawnDistanceFromPlayer;
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
        //Animate constantly
        UpdateAnimation();
    }
}
