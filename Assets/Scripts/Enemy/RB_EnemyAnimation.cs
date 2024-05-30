using UnityEngine;

public class RB_EnemyAnimation : MonoBehaviour
{
    //Components
    Animator _enemyAnimator;
    Rigidbody _rb;
    Transform _transform;
    

    private void Awake()
    {
        _enemyAnimator = GetComponent<Animator>();
        _rb = GetComponentInParent<Rigidbody>();
        _transform = _rb.transform;
    }
    private void UpdateAnim()
    {
        _enemyAnimator.SetFloat("Horizontal", _transform.forward.normalized.x);
        _enemyAnimator.SetFloat("Vertical", _transform.forward.normalized.z);
        _enemyAnimator.SetFloat("Speed", _rb.velocity.magnitude);
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
