using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_AICheck_LookForDistractions : RB_BTNode
{
    RB_AI_BTTree _btParent;
    Transform _transform;

    public RB_AICheck_LookForDistractions(RB_AI_BTTree btParent)
    {
        _btParent = btParent;
        _transform = btParent.transform;
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;

        foreach(Collider collider in Physics.OverlapSphere(_transform.position, _btParent.FovRange, (1 << 12)))
        {
            Vector3 targetDir = collider.transform.position - _transform.position;
            float angle = Vector3.Angle(targetDir, _transform.forward);
            if (angle >= -_btParent.FovAngle / 2 && angle <= _btParent.FovAngle / 2 && !Physics.Raycast(_transform.position, targetDir, targetDir.magnitude, (1 << 3)))
            {
                _btParent.AddDistraction(collider.GetComponent<RB_Distraction>());
            }
        }

        return _state;
    }
}
