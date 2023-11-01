using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class Attack : Node
{
    public AIController agent;
    public CharacterCombat.AttackType attackType;

    /// <summary>
    /// Commands an agent to make a melee attack against its target
    /// </summary>
    /// <param name="agent">The agent this command is given to</param>
    public Attack(AIController agent, CharacterCombat.AttackType attackType)
    {
        this.agent = agent;
        this.attackType = attackType;
    }

    public override NodeState Evaluate()
    {
        //Debug.Log("Attack state is attempted at:" + attackType);
        bool attackSuccess = agent.AttackTarget(attackType);
        if (attackSuccess)
        {
            //Debug.Log("Attack state is successful");
            return NodeState.Success;
        }

        //Debug.Log("Attack state failed");
        return NodeState.Failure;
    }
}
