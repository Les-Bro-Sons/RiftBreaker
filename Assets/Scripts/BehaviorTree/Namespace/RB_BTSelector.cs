using System.Collections.Generic;

namespace BehaviorTree
{
    public class RB_BTSelector : RB_BTNode
    {
        // Constructor for a selector node without children
        public RB_BTSelector() : base() { }

        // Constructor for a selector node with children
        public RB_BTSelector(List<RB_BTNode> children) : base(children) { }

        // Evaluate the selector node
        public override BTNodeState Evaluate()
        {
            // Iterate through each child node
            foreach (RB_BTNode BTNode in _children)
            {
                switch (BTNode.Evaluate())
                {
                    case BTNodeState.FAILURE:
                        continue;  // Move to the next child if the current one fails

                    case BTNodeState.SUCCESS:
                        _state = BTNodeState.SUCCESS;
                        return _state;  // Return SUCCESS if any child succeeds

                    case BTNodeState.RUNNING:
                        _state = BTNodeState.RUNNING;
                        return _state;  // Return RUNNING if any child is still running

                    default:
                        continue;  // Continue to the next child for any other state
                }
            }

            _state = BTNodeState.FAILURE;
            return _state;  // Return FAILURE if all children fail
        }
    }
}
