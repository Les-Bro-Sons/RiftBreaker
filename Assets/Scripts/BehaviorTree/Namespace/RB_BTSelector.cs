using System.Collections.Generic;

namespace BehaviorTree
{
    public class RB_BTSelector : RB_BTNode
    {
        public RB_BTSelector() : base() { }
        public RB_BTSelector(List<RB_BTNode> children) : base(children) { }

        public override BTNodeState Evaluate()
        {
            foreach (RB_BTNode BTNode in _children)
            {
                switch (BTNode.Evaluate())
                {
                    case BTNodeState.FAILURE:
                        continue;

                    case BTNodeState.SUCCESS:
                        _state = BTNodeState.SUCCESS;
                        return _state;

                    case BTNodeState.RUNNING:
                        _state = BTNodeState.RUNNING;
                        return _state;

                    default:
                        continue;
                }
            }

            _state = BTNodeState.FAILURE;
            return _state;
        }
    }
}