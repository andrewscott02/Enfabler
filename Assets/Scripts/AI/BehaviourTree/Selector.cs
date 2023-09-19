using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    public class Selector : Node
    {
        public Selector() : base() { }

        public Selector(params Node[] children) : base()
        {
            base.children.AddRange(children);
        }

        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.Failure:
                        continue;
                    case NodeState.Success:
                        state = NodeState.Success;
                        return state;
                    case NodeState.Running:
                        state = NodeState.Running;
                        return state;
                    default:
                        continue;
                }
            }

            state = NodeState.Failure;
            return state;
        }
    }
}