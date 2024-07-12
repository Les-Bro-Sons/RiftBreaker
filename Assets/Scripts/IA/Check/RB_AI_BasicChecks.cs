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

public class RB_AICheck_Random : RB_BTNode
{
    private float _percentage;
    private float _delay;

    private float _lastTimeChecked;

    /// <summary>
    /// Check if the random value is below percentage and return success if it is
    /// </summary>
    /// <param name="percentage">percentage between 0 & 1</param>
    /// <param name="delay">delay each time the random is checked</param>
    public RB_AICheck_Random(float percentage, float delay)
    {
        _percentage = percentage;
        _delay = delay;
    }

    public override BTNodeState Evaluate()
    {
        _state = BTNodeState.FAILURE;

        if (Time.time > _lastTimeChecked + _delay)
        {
            float random = Random.value;
            _lastTimeChecked = Time.time;

            if (random >= _percentage)
            {
                _state = BTNodeState.SUCCESS;
            }
        }


        return _state;
    }
}