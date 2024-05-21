using UnityEngine;

namespace BehaviorTree
{
    public abstract class RB_BTTree : MonoBehaviour
    {
        private RB_BTNode _root = null;

        protected void Start()
        {
            _root = SetupTree();
        }

        private void Update()
        {
            if (_root != null)
                _root.Evaluate();
        }

        protected abstract RB_BTNode SetupTree();
    }
}