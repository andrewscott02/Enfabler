using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    /// <Behaviour Tree Examples>
    /// https://github.falmouth.ac.uk/Joseph-WaltonRivers/comp280-pacman-unity/tree/main/Assets/Scripts/bt
    /// https://www.youtube.com/watch?v=aR6wt5BlE-E
    /// </summary>

    public enum NodeState
    {
        Running, Success, Failure
    }

    public class Node
    {
        protected NodeState state;

        public Node parent;
        protected List<Node> children;

        #region Setup

        public Node()
        {
            parent = null;
        }
        public Node(List<Node> attachChildren)
        {
            foreach (Node child in attachChildren)
            {
                AttachNode(child);
            }
        }

        private void AttachNode(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        #endregion

        public virtual NodeState Evaluate()
        {
            return NodeState.Failure;
        }
    }
}
