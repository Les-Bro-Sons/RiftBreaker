using BehaviorTree;
using System.Collections;
using UnityEngine;

/*public class RB_AIInf_Attack : RB_BTNode
{
    private RB_AIInf_BTTree _btParent;
    private Transform _transform;

    private float _attackCounter = 0f;
    private float _waitBeforeAttackCounter = 0f;
    
    private bool _hasAlreadyInit = false;
    
    //private Animator _animator;

    public RB_AIInf_Attack(RB_AIInf_BTTree BtParent)
    {
        _btParent = BtParent;
        _transform = _btParent.transform;
        // _animator = transform.GetComponent<Animator>();
    }

    public override BTNodeState Evaluate()
    {
        Transform target = (Transform)GetData("target");

        if (target == null)
        {
            _state = BTNodeState.FAILURE;
            return _state;
        }

        if (!_hasAlreadyInit)
        {
            _hasAlreadyInit = true;
        }


        Vector3 direction = target.position - _transform.position;
        float distance = direction.magnitude;
        if (distance <= _btParent.AttackRange)
        { 
            _btParent.IsAttacking = true;
            
            _waitBeforeAttackCounter += Time.deltaTime;
            if (_waitBeforeAttackCounter >= _btParent.WaitBeforeAttack)
            {
                // Remove the distance check
                if (target != null)
                {
                    ActionAttack(target);
                    /*
                    if (!_btParent.IsAttacking)
                    {
                        _attackCoroutine = _btParent.StartCoroutine(AttackCoroutine(target));
                    }
                    */
/*
                }
            }
        }

        Debug.Log($"_attackCounter : {_attackCounter}");
        _state = BTNodeState.RUNNING;
        return _state;
    }

    private IEnumerator AttackCoroutine(Transform target)
    {
        _btParent.IsAttacking = true;
        yield return new WaitForSeconds(_btParent.WaitBeforeAttack);


        if (target != null)
        {
            Vector3 direction = target.position - _transform.position;
            float distance = direction.magnitude;
            if (distance <= _btParent.AttackRange)
            {
                RB_Tools.TryGetComponentInParent<RB_Health>(target.gameObject, out RB_Health _targetHealth);
                _targetHealth.TakeDamage(_btParent.AttackDamage = _targetHealth.HpMax);
                Debug.Log("Attack!");
            }

            // play animation
            Debug.Log(" HIT ");
        }
        else
        {
            ClearData("target");
            Debug.LogWarning("ClearData: target");
            _hasAlreadyInit = false;
        }        
    }

    private void ActionAttack(Transform target)
    {
        
        _attackCounter += Time.deltaTime;
        if (_attackCounter >= _btParent.AttackSpeed)
        {
            if (target != null)
            {
                Vector3 direction = target.position - _transform.position;
                float distance = direction.magnitude;
                if (distance <= _btParent.AttackRange)
                {
                    RB_Tools.TryGetComponentInParent<RB_Health>(target.gameObject, out RB_Health _targetHealth);
                    _targetHealth.TakeDamage(_btParent.AttackDamage = _targetHealth.HpMax);
                    Debug.Log("Attack!");
                }

                // play animation
                Debug.Log(" HIT ");
                _attackCounter = 0f;
                _waitBeforeAttackCounter = 0f;
            }
            else
            {
                ClearData("target");
                Debug.LogWarning("ClearData: target");
            }
            
            _btParent.IsAttacking = false;
            _hasAlreadyInit = false;
        }
    }
}*/
