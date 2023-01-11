using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class BasicBehaviourTree : BehaviourTree
{
    protected override Node SetupTree()
    {
        //Node root = new Roam(agent, 30f, 1f, 6f);

        Node root = new Sequence(new List<Node>
        {
            new FindPointRadius(agent, 30f),
            new MoveToDestination(agent, 1f, 6f)
        });

        return root;
    }
}
