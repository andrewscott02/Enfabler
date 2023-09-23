using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class FlankToDestination : Node
{
    public AIController agent;
    public BaseCharacterController flankTarget;
    public float distance;
    public bool requiresSameTeam;

    /// <summary>
    /// Commands an agent to flank its current target with another character's position
    /// </summary>
    /// <param name="agent"The agent this command is given to></param>
    /// <param name="flankTarget">The target of the flank</param>
    /// <param name="distance">The distance the agent will try to keep on the other side of the flank target</param>
    /// <param name="requiresSameTeam">Whether this requires the agent and intercept target to be on the same team</param>
    public FlankToDestination(AIController agent, GameObject flankTarget, float distance, bool requiresSameTeam)
    {
        this.agent = agent;
        this.flankTarget = flankTarget.GetComponent<BaseCharacterController>();
        this.distance = distance;
        this.requiresSameTeam = requiresSameTeam;
    }

    public override NodeState Evaluate()
    {
        if (flankTarget == null) { return NodeState.Failure; }

        if (requiresSameTeam && AIManager.instance.OnSameTeam(agent, flankTarget) == false) { return NodeState.Failure; }

        float flankDistance = Mathf.Clamp(distance, 0, agent.maxDistanceFromModelCharacter);

        Vector3 point = HelperFunctions.GetFlankingPoint(flankTarget.transform.position, agent.currentTarget.transform.position, flankDistance);

        agent.SetDestinationPos(point);
        //Debug.Log("Generated point near target at: " + point);

        agent.roaming = true;
        state = NodeState.Success;
        return state;
    }
}