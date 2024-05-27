using Unity.VisualScripting;
using UnityEngine;

public class RB_PlayerAnim : MonoBehaviour
{
    //Components
    Animator _playerAnimator;
    [SerializeField] RB_PlayerMovement _playerMovement;
    private Transform _playerTransform;
    private RB_PlayerAction _playerAction;

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
            _playerAnimator.SetFloat("Horizontal", _playerMovement.GetDirectionToMove().x);
            _playerAnimator.SetFloat("Vertical", _playerMovement.GetDirectionToMove().z);
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
        //Spawn the prefab by his name
        GameObject newObject = Instantiate(Resources.Load("Prefabs/Projectiles/" + prefabToSpawn), _playerTransform.position + new Vector3(0, -.5f, 0), _playerTransform.rotation) as GameObject;
        if (newObject.TryGetComponent<RB_Projectile>(out RB_Projectile projectile))
        {
            projectile.Team = TEAMS.Player;
        }
    }

    private void Update()
    {
        //Animate constantly
        UpdateAnimation();
    }
}
