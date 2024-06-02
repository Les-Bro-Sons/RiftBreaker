using UnityEngine;
using UnityEngine.Events;

public class RB_LevelTrigger : MonoBehaviour
{
    [SerializeField] private bool _triggerOnPlayer = true;
    [SerializeField] private bool _oneTimeOnly = true;

    public UnityEvent EventTriggerEnter;
    public UnityEvent EventTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if ((_triggerOnPlayer && RB_Tools.TryGetComponentInParent<RB_PlayerController>(other.gameObject, out RB_PlayerController playerController)) || !_triggerOnPlayer)
        {
            EventTriggerEnter?.Invoke();
            if (_oneTimeOnly)
            {
                Destroy(gameObject);
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if ((_triggerOnPlayer && RB_Tools.TryGetComponentInParent<RB_PlayerController>(other.gameObject, out RB_PlayerController playerController)) || !_triggerOnPlayer)
        {
            EventTriggerExit?.Invoke();
            if (_oneTimeOnly)
            {
                Destroy(gameObject);
            }
        }
    }
}
