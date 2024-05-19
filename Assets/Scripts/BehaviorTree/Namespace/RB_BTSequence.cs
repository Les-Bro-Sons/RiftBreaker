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

            foreach (RB_BTNode BTNode in children)
            {
                switch (BTNode.Evaluate())
                {
                    case BTNodeState.FAILURE:
                        state = BTNodeState.FAILURE;
                        return state;

                    case BTNodeState.SUCCESS:
                        continue;

                    case BTNodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;

                    default:
                        state = BTNodeState.SUCCESS;
                        return state;
                }
            }

            state = anyChildIsRunning ? BTNodeState.RUNNING : BTNodeState.SUCCESS;
            return state;
        }
    }
}