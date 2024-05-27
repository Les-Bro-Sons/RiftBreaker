using BehaviorTree;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.UI;

public class RB_AIInf_GoToTarget : RB_BTNode
{
    private RB_AIInf_BTTree _btParent;

    private Transform _transform;
    //private Animator _animator;

    public RB_AIInf_GoToTarget(RB_AIInf_BTTree BtParent)
    {
        _btParent = BtParent;
        _transform = _btParent.transform;
        // _animator = transform.GetComponent<Animator>();
    }

    public override BTNodeState Evaluate()
    {
        if (_btParent.IsAttacking)
        {
            _state = BTNodeState.SUCCESS;
            return _state;
        }
        else
        {
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
                //_transform.LookAt(target);
                _btParent.AiMovement.MoveIntoDirection(direction, _btParent.MovementSpeedAttack);
                _state = BTNodeState.RUNNING;
            }

            return _state;
        }
    }
}