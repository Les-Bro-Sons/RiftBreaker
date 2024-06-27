using UnityEngine;

namespace BehaviorTree
{
    public abstract class RB_BTTree : MonoBehaviour
    {
        private RB_BTNode _root = null; // The root node of the behavior tree
        public RB_BTNode Root { get { return _root; } } // Public property to access the root node

        // Called when the script instance is being loaded
        protected virtual void Start()
        {
            _root = SetupTree(); // Setup the behavior tree and assign the root node
        }

        // Called once per frame
        protected virtual void Update()
        {
            if (_root != null)
                _root.Evaluate(); // Evaluate the behavior tree starting from the root node
        }

        // Abstract method to be implemented by derived classes to setup the behavior tree
        protected abstract RB_BTNode SetupTree();
    }
}
