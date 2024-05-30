using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_AI_DoFailure : RB_BTNode
{
    public override BTNodeState Evaluate()
    {
        return _state = BTNodeState.FAILURE;
    }
}
