using BehaviorTree;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class RB_AI_StaticWatchOut : RB_BTNode
{
    private Transform _transform;
    private RB_AI_BTTree _btParent;
    private Rigidbody _rb;
    private RB_AiMovement _btMovement;

    private float _distanceRequired;

    public RB_AI_StaticWatchOut(RB_AI_BTTree BtParent, float distanceRequired = 2)
    {
        _btParent = BtParent;
        _transform = BtParent.transform;
        _rb = _btParent.AiRigidbody;
        _distanceRequired = distanceRequired;
        _btMovement = _btParent.AiMovement;
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;
        if (!_btParent.IsStatic) return _state = BTNodeState.FAILURE;

        if (_btParent.IsOnStaticPoint) //is on point
        {
            if (Vector3.Distance(_transform.position, _btParent.StaticPosition) > _distanceRequired)
            {
                _btParent.IsOnStaticPoint = false;
            }
        }
         

        if (!_btParent.IsOnStaticPoint) //is not on point
        {
            if (Vector3.Distance(_transform.position, _btParent.StaticPosition) <= _distanceRequired)
            {
                _rb.MoveRotation(Quaternion.LookRotation(_btParent.StaticLookDirection));
                _btParent.IsOnStaticPoint = true;
            }
            else
            {
                _btMovement.MoveToPosition(_btParent.StaticPosition, _btParent.MovementSpeed);
            }
        }
        

        return _state = BTNodeState.SUCCESS;
    }
}
