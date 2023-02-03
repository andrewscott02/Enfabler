using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class CheckEqual<T> : Node
{
    public T a;
    public IntervalBehaviourTree bt;

    /// <summary>
    /// Checks the current state of the interval behaviour tree
    /// </summary>
    /// <param name="a">The value required</param>
    /// <param name="bt">The interval behaviour tree</param>
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
