using System.Collections.Generic;

namespace BehaviorTree
{
    public class RB_BTSelector : RB_BTNode
    {
        public RB_BTSelector() : base() { }
        public RB_BTSelector(List<RB_BTNode> children) : base(children) { }

        public override BTNodeState Evaluate()
        {
            foreach (RB_BTNode BTNode in children)
            {
                switch (BTNode.Evaluate())
                {
                    case BTNodeState.FAILURE:
                        continue;

                    case BTNodeState.SUCCESS:
                        state = BTNodeState.SUCCESS;
                        return state;

                    case BTNodeState.RUNNING:
                        state = BTNodeState.RUNNING;
                        return state;

                    default:
                        continue;
                }
            }

            state = BTNodeState.FAILURE;
            return state;
        }
    }
}