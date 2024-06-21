using BehaviorTree;
using Unity.VisualScripting;
using UnityEngine;

public class RB_AI_BecomeDecoration : RB_BTNode
{
    private RB_AI_BTTree _btParent;
    private Transform _transform;
    private Rigidbody _rb;

    private RigidbodyConstraints _oldConstraints;

    public RB_AI_BecomeDecoration(RB_AI_BTTree btParent)
    {
        _btParent = btParent;
        _transform = _btParent.transform;
        _rb = _btParent.AiRigidbody;
        RB_LevelManager.Instance.EventSwitchPhase.AddListener(CheckForUnDecorate);
        _oldConstraints = _rb.constraints;
    }
    public override BTNodeState Evaluate()
    {
        _btParent.AiAnimator.SetBool("IsDecoration", true);
        _rb.constraints = RigidbodyConstraints.FreezeAll;
        _rb.MoveRotation(Quaternion.LookRotation(_btParent.StaticLookDirection));
        return BTNodeState.SUCCESS;
    }

    public void CheckForUnDecorate()
    {
        if (_btParent.IsDecorative && (RB_LevelManager.Instance.CurrentPhase == PHASES.Combat || RB_LevelManager.Instance.CurrentPhase == PHASES.Boss))
        {
            _btParent.AiAnimator.SetBool("IsDecoration", false);
            _rb.constraints = _oldConstraints;
        }
    }
}
