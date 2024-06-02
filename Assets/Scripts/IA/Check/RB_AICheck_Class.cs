using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_AICheck_Class : RB_BTNode
{
    ENEMYCLASS _parentClass;
    ENEMYCLASS _checkingClass;


    public RB_AICheck_Class(ENEMYCLASS parentClass, ENEMYCLASS checkingClass)
    {
        _parentClass = parentClass;
        _checkingClass = checkingClass;
    }

    public override BTNodeState Evaluate()
    {
        if (_parentClass == _checkingClass)
        {
            _state = BTNodeState.SUCCESS;
            return _state;
        }
        else
        {
            _state = BTNodeState.FAILURE;
            return _state;
        }
    }
}
