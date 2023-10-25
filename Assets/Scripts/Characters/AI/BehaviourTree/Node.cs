using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    /// <Behaviour Tree Examples>
    /// https://github.falmouth.ac.uk/Joseph-WaltonRivers/comp280-pacman-unity/tree/main/Assets/Scripts/bt
    /// https://www.youtube.com/watch?v=aR6wt5BlE-E
    /// https://hub.packtpub.com/building-your-own-basic-behavior-tree-tutorial/
    /// </summary>

    public enum NodeState
    {
        Running, Success, Failure
    }

    public class Node
    {
        protected NodeState state;

        public Node parent;
        protected List<Node> children = new List<Node>();

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

        #region Data
        
        private Dictionary<string, object> dataContext = new Dictionary<string, object>();

        public void SetData(string key, object value)
        {
            dataContext[key] = value;
        }

        public object GetData(string key)
        {
            object value = null;
            if (dataContext.TryGetValue(key, out value))
                return value;

            Node node = parent;
            while (node != null)
            {
                value = node.GetData(key);
                if (value != null)
                    return value;
                node = node.parent;
            }
            return null;
        }

        public bool ClearData(string key)
        {
            if (dataContext.ContainsKey(key))
            {
                dataContext.Remove(key);
                return true;
            }

            Node node = parent;
            while (node != null)
            {
                bool cleared = node.ClearData(key);
                if (cleared)
                    return true;
                node = node.parent;
            }
            return false;
        }
        
        #endregion
    }
}
