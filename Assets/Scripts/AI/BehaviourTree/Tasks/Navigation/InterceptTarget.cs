using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class InterceptTarget : Node
{
    public AIController agent;
    public CharacterController interceptTarget;
    public float distance;
    public bool requiresSameTeam;

    /// <summary>
    /// Commands an agent to intercept itself between a specified target and the agent's current target
    /// </summary>
    /// <param name="agent"The agent this command is given to></param>
    /// <param name="interceptTarget">The target of the intercept</param>
    /// <param name="distance">The distance the agent will try to keep near the intercept target</param>
    /// <param name="requiresSameTeam">Whether this requires the agent and intercept target to be on the same team</param>
    public InterceptTarget(AIController agent, GameObject interceptTarget, float distance, bool requiresSameTeam)
    {
        this.agent = agent;
        this.interceptTarget = interceptTarget.GetComponent<CharacterController>();
        this.distance = distance;
        this.requiresSameTeam = requiresSameTeam;
    }

    public override NodeState Evaluate()
    {
        if (interceptTarget == null) { return NodeState.Failure; }

        if (requiresSameTeam && AIManager.instance.OnSameTeam(agent, interceptTarget) == false) { return NodeState.Failure; }

        float interceptDistance = -Mathf.Clamp(distance, 0, agent.maxDistanceFromModelCharacter);

        Vector3 point = HelperFunctions.GetFlankingPoint(agent.currentTarget.transform.position, interceptTarget.transform.position, interceptDistance);

        agent.SetDestinationPos(point);
        //Debug.Log("Generated point near target at: " + point);

        agent.roaming = true;
        state = NodeState.Success;
        return state;
    }
}