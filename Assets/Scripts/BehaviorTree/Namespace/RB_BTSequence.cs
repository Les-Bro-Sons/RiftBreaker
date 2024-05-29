using System.Collections.Generic;

namespace BehaviorTree
{
    public class RB_BTSequence : RB_BTNode
    {
        public RB_BTSequence() : base() { }
        public RB_BTSequence(List<RB_BTNode> children) : base(children) { }

        public override BTNodeState Evaluate()
        {
            bool anyChildIsRunning = false;

            foreach (RB_BTNode BTNode in _children)
            {
                if (anyChildIsRunning)
                {
                    break;
                }
                switch (BTNode.Evaluate())
                {
                    case BTNodeState.FAILURE:
                        _state = BTNodeState.FAILURE;
                        return _state;

                    case BTNodeState.SUCCESS:
                        continue;

                    case BTNodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;

                    default:
                        _state = BTNodeState.SUCCESS;
                        return _state;
                }
            }

            _state = anyChildIsRunning ? BTNodeState.RUNNING : BTNodeState.SUCCESS;
            return _state;
        }
    }
}