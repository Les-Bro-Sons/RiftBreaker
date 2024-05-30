using BehaviorTree;
using UnityEngine;

public class RB_AI_GoToTarget : RB_BTNode
{
    private RB_AI_BTTree _btParent;

    private Transform _transform;

    private float _range;
    private float _speed;

    public RB_AI_GoToTarget(RB_AI_BTTree BtParent, float speed, float range)
    {
        _btParent = BtParent;
        _transform = _btParent.transform;
        _range = range;
        _speed = speed;
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

        if (distance <= _range) // Vérifie si l'agent est suffisamment proche de la cible
        {
            _state = BTNodeState.SUCCESS;
        }
        else
        {
            direction.Normalize();
            //_transform.position += direction * _btParent.MovementSpeedAttack * Time.deltaTime; // Déplacement de l'agent vers la cible

            //_btParent.AiMovement.MoveIntoDirection(direction, _btParent.MovementSpeedAttack);
            _btParent.AiMovement.MoveToPosition(target.position, _speed);

            //_transform.LookAt(target);
            _state = BTNodeState.RUNNING;
        }

        return _state;
    }
}