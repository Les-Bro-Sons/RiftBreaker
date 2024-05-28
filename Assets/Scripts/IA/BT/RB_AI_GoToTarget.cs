using BehaviorTree;
using UnityEngine;

public class RB_AI_GoToTarget : RB_BTNode
{
    private RB_AI_BTTree _btParent;

    private Transform _transform;
    //private Animator _animator;

    public RB_AI_GoToTarget(RB_AI_BTTree BtParent)
    {
        _btParent = BtParent;
        _transform = _btParent.transform;
        // _animator = transform.GetComponent<Animator>();
    }

    public override BTNodeState Evaluate()
    {
        if (_btParent.IsAttacking) return _state = BTNodeState.SUCCESS;

        Transform target = (Transform)GetData("target");

        if (target == null)
        {
            _state = BTNodeState.FAILURE;
            return _state;
        }

        Vector3 direction = target.position - _transform.position;
        float distance = direction.magnitude;

        //Debug.Log($"distance : {distance}");

        if (distance <= _btParent.AttackRange) // Vérifie si l'agent est suffisamment proche de la cible
        {
            _state = BTNodeState.SUCCESS;
        }
        else
        {
            direction.Normalize();
            //_transform.position += direction * _btParent.MovementSpeedAttack * Time.deltaTime; // Déplacement de l'agent vers la cible
            _btParent.AiMovement.MoveIntoDirection(direction, _btParent.MovementSpeedAttack);
            //_transform.LookAt(target);
            _state = BTNodeState.RUNNING;
        }

        return _state;
    }
}