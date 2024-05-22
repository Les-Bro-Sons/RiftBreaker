using BehaviorTree;
using UnityEngine;

public class RB_AI_PlayerInRange : RB_BTNode
{
    private Transform _transform;
    private static int _layerMask = 1 << 7;
    //private Animator _animator;

    public RB_AI_PlayerInRange(Transform transform)
    {
        _transform = transform;
        //Debug.Log("PlayerInRange" + _transform);
        // _animator = transform.GetComponent<Animator>();
    }

    public override BTNodeState Evaluate()
    {
        object t = GetData("target");
        if (t == null)
        {
            Collider[] colliders = Physics.OverlapSphere(
                _transform.position, RB_AIInf_BTTree.FovRange, _layerMask);

            if (colliders.Length > 0)
            {
                Parent.Parent.SetData("target", colliders[0].transform);
            }

            _state = BTNodeState.RUNNING;
            return _state;
        }



        

        Transform target = (Transform)t;

        if (Vector3.Distance(_transform.position, target.position) <= RB_AIInf_BTTree.FovRange)
        {
            // _animator.SetBool("Attacking", true);
            // _animator.SetBool("Walking", false);

            _state = BTNodeState.SUCCESS;
            return _state;
        }

        _state = BTNodeState.FAILURE;
        return _state;
    }
}