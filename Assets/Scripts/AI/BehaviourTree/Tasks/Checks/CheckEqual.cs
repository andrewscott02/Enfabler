using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class CheckEqual<T> : Node
{
    public T a;
    public IntervalBehaviourTree bt;

    /// <summary>
    /// Commands an agent to roam to a random point within a specified radius
    /// </summary>
    /// <param name="newAgent">The agent this command is given to</param>
    /// <param name="radius">The radius of the roam position, recommend 30</param>
    public CheckEqual(T a, IntervalBehaviourTree bt)
    {
        this.a = a;
        this.bt = bt;
    }

    public override NodeState Evaluate()
    {
        state = a.Equals(bt.currentState) ? NodeState.Success : NodeState.Failure;
        Debug.Log("Check equal returned " + state + " for " + a + " and " + bt.currentState);
        return state;
    }
}
