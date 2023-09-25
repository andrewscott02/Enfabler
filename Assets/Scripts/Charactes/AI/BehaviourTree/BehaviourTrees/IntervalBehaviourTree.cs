using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;

public class IntervalBehaviourTree : BehaviourTree
{
    public ConstructPlayerModel playerModel;
    public int currentState = 0;
    int maxState = 4;
    public Vector2 switchInterval = new Vector2(3, 8);

    protected override Node SetupTree()
    {
        DelaySwitchState(switchInterval);

        //Debug.Log("Setting up adaptive BT for " + agent.name);

        Node root = new Selector(

            //If agent is too far away from model character, rush to a distance within range
            new Sequence(
                new CheckOutDistance(agent, playerModel, agent.maxDistanceFromModelCharacter),
                new Selector(
                    BaseBehaviours.RushToTarget(agent, playerModel.modelCharacter)
                    )
                ),

        #region State Behaviours

            //In state 0, focus on enemies they are not targetting
            new Sequence(
                new CheckEqual<int>(0, this),
                new Selector(
                    BaseBehaviours.IgnoreModelTargets(agent, playerModel),
                    //If player is targetting all available targets, move to closest target and attack
                    BaseBehaviours.AttackClosestTarget(agent),
                    BaseBehaviours.MoveToClosestTarget(agent, agent.distanceAllowance, true)
                    )
                ),
            //In state 1, move slowly to player and attack enemies around them
            new Sequence(
                new CheckEqual<int>(1, this),
                BaseBehaviours.MoveToTargetWhileAttacking(agent, playerModel.modelCharacter, agent.distanceAllowance)
                ),
            //In state 2, rush to player and attack enemies around them
            new Sequence(
                new CheckEqual<int>(2, this),
                BaseBehaviours.RushToTarget(agent, playerModel.modelCharacter)
                ),
            //In state 3, draw enemies away from them
            new Sequence(
                new CheckEqual<int>(3, this),
                BaseBehaviours.FlankTarget(agent, playerModel, agent.meleeDistance, true, true)
                ),
            //In state 4, intercept between player and their closest target
            new Sequence(
                new CheckEqual<int>(4, this),
                BaseBehaviours.InterceptTarget(agent, playerModel, agent.meleeDistance, true, true)
                ),

        #endregion

        #region General Behaviours - In case of failure

            //Checks if the closest enemy is within melee range and makes an attack if true
            BaseBehaviours.AttackClosestTarget(agent),
            //Checks if the closest enemy is within sight range and moves towards it if true
            BaseBehaviours.MoveToClosestTarget(agent, agent.distanceAllowance, true),

        #endregion

        #region Idle Behaviours - When there are no enemies

            //If there are no targets, but the player is an ally, move to a point near the player
            BaseBehaviours.FollowTarget(agent, agent.GetPlayer(), true),
            //If there are no targets, move to a random point in the roam radius
            BaseBehaviours.RoamToRandomPoint(agent)

        #endregion

            );

        return root;
    }

    void DelaySwitchState(Vector2 delayV2)
    {
        float delay = Random.Range(delayV2.x, delayV2.y);
        Debug.Log("Switching state in " + delay);
        Invoke("SwitchState", delay);
    }

    void SwitchState()
    {
        currentState = Random.Range(0, maxState);
        Debug.Log("Switching state now to " + currentState);
        DelaySwitchState(switchInterval);
    }

    IEnumerator SwitchState(Vector2 delayV2)
    {
        float delay = Random.Range(delayV2.x, delayV2.y);

        yield return new WaitForSeconds(delay);

        currentState++;
        if (currentState > maxState) { currentState = 0; }
        StartCoroutine(SwitchState(switchInterval));
    }
}