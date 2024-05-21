using UnityEngine;

public class RB_Enemy : MonoBehaviour
{
    protected virtual void Awake()
    {
        GetComponent<RB_Health>().EventDeath.AddListener(Death);
        GetComponent<RB_Health>().EventTakeDamage.AddListener(TakeDamage);
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void TakeDamage()
    {

    }

    protected virtual void Death()
    {
        Destroy(gameObject);
    }
}
