using System.Collections.Generic;

namespace BehaviorTree
{
    public enum BTNodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class RB_BTNode
    {
        protected BTNodeState _state;

        public RB_BTNode Parent;
        protected List<RB_BTNode> _children = new();

        private Dictionary<string, object> _dataContext = new();

        public RB_BTNode()
        {
            Parent = null;
        }

        public RB_BTNode(List<RB_BTNode> children)
        {
            foreach (RB_BTNode child in children)
                _Attach(child);
        }

        private void _Attach(RB_BTNode BTNode)
        {
            BTNode.Parent = this;
            _children.Add(BTNode);
        }

        public virtual BTNodeState Evaluate() => BTNodeState.FAILURE;

        public void SetData(string key, object value)
        {
            _dataContext[key] = value;
        }

        public object GetData(string key)
        {
            object value = null;
            if (_dataContext.TryGetValue(key, out value))
                return value;

            RB_BTNode BTNode = Parent;
            while (BTNode != null)
            {
                value = BTNode.GetData(key);
                if (value != null)
                    return value;
                BTNode = BTNode.Parent;
            }
            return null;
        }

        public bool ClearData(string key)
        {
            if (_dataContext.ContainsKey(key))
            {
                _dataContext.Remove(key);
                return true;
            }

            RB_BTNode BTNode = Parent;
            while (BTNode != null)
            {
                bool cleared = BTNode.ClearData(key);
                if (cleared)
                    return true;
                BTNode = BTNode.Parent;
            }
            return false;
        }
    }
}