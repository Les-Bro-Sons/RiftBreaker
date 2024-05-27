using BehaviorTree;
using UnityEngine;
using UnityEngine.UIElements;
using static RB_AICombat_BTTree;

public class RB_AICombat_Attack : RB_BTNode
{
    private RB_AICombat_BTTree _btParent;
    private Transform _transform;

    private float _attackCounter = 0f;
    private RB_Health _targetHealth;
    private bool _hasAlreadyInit = false;

    //private Animator _animator;

    public RB_AICombat_Attack(RB_AICombat_BTTree BtParent)
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

        _attackCounter += Time.deltaTime;
        if (_attackCounter >= _btParent.AttackSpeed)
        {
            if (target != null)
            {
                Vector3 direction = target.position - _transform.position;
                float distance = direction.magnitude;

                switch (_btParent.AiType)
                {
                    case AI_Type.FAIBLE:
                        if (distance <= _btParent.AttackRange) // Vérifie si l'agent est suffisamment proche de la cible
                        {                    
                            RB_Tools.TryGetComponentInParent<RB_Health>(target.gameObject, out RB_Health _targetHealth); // A REMPLACER QUAND IL Y AURA UNE ANIMATION
                            _targetHealth.TakeDamage(_btParent.AttackDamage);
                            _attackCounter = 0f;
                        }
                        break;
                    
                    case AI_Type.MOYEN:
                        break;
                    
                    case AI_Type.FORT:
                        break;

                    default: 
                        //_state = BTNodeState.FAILURE;
                        break;
                }
            }
            else
            {
                ClearData("target");
                Debug.LogWarning("ClearData : target");
                _hasAlreadyInit = false;
            }
        }

        _state = BTNodeState.RUNNING;
        return _state;
    }
}
