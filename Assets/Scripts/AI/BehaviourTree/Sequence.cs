using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    public class Sequence : Node
    {
        public Sequence() : base() { }
        public Sequence(List<Node> children) :base(children) { }

        public override NodeState Evaluate()
        {
            bool childRunning = false;

            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.Failure:
                        state = NodeState.Failure;
                        return state;
                    case NodeState.Success:
                        continue;
                    case NodeState.Running:
                        childRunning = true;
                        continue;
                    default:
                        state = NodeState.Success;
                        return state;
                }
            }

            state = childRunning ? NodeState.Running : NodeState.Success;
            return state;
        }
    }
}
