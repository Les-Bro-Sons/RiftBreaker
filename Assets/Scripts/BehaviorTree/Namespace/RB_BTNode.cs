using System.Collections.Generic;

namespace BehaviorTree
{
    // Enum to represent the state of a behavior tree node
    public enum BTNodeState
    {
        RUNNING,  // Node is currently running
        SUCCESS,  // Node completed successfully
        FAILURE   // Node failed to complete
    }

    public class RB_BTNode
    {
        protected BTNodeState _state;  // The current state of the node

        public RB_BTNode Parent;  // Reference to the parent node
        protected List<RB_BTNode> _children = new();  // List of child nodes

        private Dictionary<string, object> _dataContext = new();  // Data context for storing key-value pairs

        // Constructor for a node without children
        public RB_BTNode()
        {
            Parent = null;
        }

        // Constructor for a node with children
        public RB_BTNode(List<RB_BTNode> children)
        {
            foreach (RB_BTNode child in children)
                _Attach(child);  // Attach each child node
        }

        // Attach a child node to this node
        private void _Attach(RB_BTNode BTNode)
        {
            BTNode.Parent = this;  // Set this node as the parent of the child
            _children.Add(BTNode);  // Add the child to the list of children
        }

        // Evaluate the node (default implementation returns FAILURE)
        public virtual BTNodeState Evaluate() => BTNodeState.FAILURE;

        // Set data in the node's context
        public void SetData(string key, object value)
        {
            _dataContext[key] = value;
        }

        // Get data from the node's context or its ancestors' contexts
        public object GetData(string key)
        {
            if (_dataContext.TryGetValue(key, out object value))
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

        // Clear data from the node's context or its ancestors' contexts
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
