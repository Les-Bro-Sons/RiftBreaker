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
            _playerAnimator.SetFloat("Horizontal", _playerMovement.GetDirectionToMove().x);
            _playerAnimator.SetFloat("Vertical", _playerMovement.GetDirectionToMove().z);
        }
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
        print("spawned one prefab");
        Instantiate(Resources.Load("Prefabs/Projectiles/" + prefabToSpawn), _playerTransform.position + new Vector3(0, -.5f, 0), _playerTransform.rotation);
    }

    private void Update()
    {
        //Animate constantly
        UpdateAnimation();
    }
}
