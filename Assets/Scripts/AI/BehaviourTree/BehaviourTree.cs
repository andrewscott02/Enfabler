using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    public abstract class BehaviourTree : MonoBehaviour
    {
        protected AIController agent;
        private Node root = null;

        public void Setup(AIController newAgent)
        {
            agent = newAgent;
            root = SetupTree();
        }

        private void Update()
        {
            if (root != null)
            {
                Debug.Log("Evaluating");
                root.Evaluate();
            }
        }

        protected abstract Node SetupTree();
    }
}