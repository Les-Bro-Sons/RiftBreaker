using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_AICheck_Comparison : RB_BTNode
{
    private RB_AI_BTTree _btParent;

    private string _variableA;
    private string _variableB;
    private float? _variableBfloat = null;
    private string _comparison;

    /// <summary>
    /// Basic comparison check
    /// </summary>
    /// <param name="btParent"></param>
    /// <param name="a">Name of variable A</param>
    /// <param name="b">Name of variable B</param>
    /// <param name="comparison">Operator (<, >, ==, etc...)</param>
    public RB_AICheck_Comparison(RB_AI_BTTree btParent, string a, string b, string comparison)
    {
        _btParent = btParent;
        _variableA = a;
        _variableB = b;
        _comparison = comparison;
    }
    
    public RB_AICheck_Comparison(RB_AI_BTTree btParent, string a, float b, string comparison)
    {
        _btParent = btParent;
        _variableA = a;
        _variableBfloat = b;
        _comparison = comparison;
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;

        float floatA = (float)_btParent.GetType().GetField(_variableA).GetValue(_btParent);
        float floatB = 0;
        if (_variableBfloat != null) floatB = _variableBfloat.Value;
        else floatB = (float)_btParent.GetType().GetField(_variableB).GetValue(_btParent);


        bool result = false;
        switch(_comparison)
        {
            case "<":
                result = floatA < floatB;
                break;
            case "<=":
                result = floatA <= floatB;
                break;
            case ">":
                result = floatA > floatB;
                break;
            case ">=":
                result = floatA >= floatB;
                break;
            case "==":
                result = floatA == floatB;
                break;
            default:
                Debug.LogError("Comparison has non recognized operator");
                break;
        }

        if (result) return _state = BTNodeState.SUCCESS;
        else return _state = BTNodeState.FAILURE;
    }
}