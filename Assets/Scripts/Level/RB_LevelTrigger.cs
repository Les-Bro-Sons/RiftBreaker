using UnityEngine;
using UnityEngine.Events;

public class RB_LevelTrigger : MonoBehaviour
{
    [SerializeField] private bool _triggerOnPlayer = true;
    [SerializeField] private bool _oneTimeOnly = true;

    [SerializeField] private bool _triggerEnterOnRewind = false;
    [SerializeField] private bool _triggerExitOnRewind = true;

    [SerializeField] private bool _triggerEnterKinematics = false;
    [SerializeField] private bool _triggerExitKinematics = true;

    public UnityEvent EventTriggerEnter;
    public UnityEvent EventTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if ((_triggerOnPlayer && RB_Tools.TryGetComponentInParent<RB_PlayerController>(other.gameObject, out RB_PlayerController playerController)) || !_triggerOnPlayer)
        {
            if (_triggerEnterKinematics || (RB_Tools.TryGetComponentInParent<Rigidbody>(other.gameObject, out Rigidbody playerBody) && playerBody.isKinematic)) return;
            
            if (_triggerEnterOnRewind || (!_triggerEnterOnRewind && !RB_TimeManager.Instance.IsRewinding))
            {
                EventTriggerEnter?.Invoke();
                if (_oneTimeOnly)
                {
                    Destroy(gameObject);
                }
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if ((_triggerOnPlayer && RB_Tools.TryGetComponentInParent<RB_PlayerController>(other.gameObject, out RB_PlayerController playerController)) || !_triggerOnPlayer)
        {
            if (_triggerExitKinematics || (RB_Tools.TryGetComponentInParent<Rigidbody>(other.gameObject, out Rigidbody playerBody) && playerBody.isKinematic)) return;

            if (_triggerExitOnRewind || (!_triggerExitOnRewind && !RB_TimeManager.Instance.IsRewinding))
            {
                EventTriggerExit?.Invoke();
                if (_oneTimeOnly)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
