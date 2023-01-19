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
                //Debug.Log("Evaluating");
                root.Evaluate();
            }
        }

        protected abstract Node SetupTree();
    }

    #region Basic Behaviour Combinations

    public static class BaseBehaviours
    {
        #region Movement

        public static Sequence RoamToRandomPoint(AIController agent)
        {
            return new Sequence(
                new FindPointRadius(agent, agent.roamDistance),
                new MoveToDestination(agent, agent.distanceAllowance, 6f, false)
                );
        }

        public static Sequence MoveToClosestTarget(AIController agent)
        {
            return new Sequence(
                new GetClosestEnemy(agent, agent.sightDistance),
                new MoveToDestination(agent, agent.distanceAllowance, 6f, false)
                );
        }

        public static Sequence FollowTarget(AIController agent, GameObject target, bool requireSameTeam)
        {
            return new Sequence(
                new FindPointNearTarget(agent, agent.GetPlayer(), agent.followDistance, requireSameTeam),
                new MoveToDestination(agent, agent.distanceAllowance, Mathf.Infinity, false));
        }

        public static Selector RushToTarget(AIController agent, GameObject target)
        {
            return new Selector(
                new Sequence(
                    new GetClosestEnemyToTarget(agent, target),
                    new MoveToDestination(agent, agent.distanceAllowance, 6f, true),
                    new MeleeAttack(agent, agent.currentTarget)
                    ),
                new Sequence(
                    new GetClosestEnemyToTarget(agent, target),
                    new MoveToDestination(agent, agent.distanceAllowance, 6f, true)
                    )
                );
        }

        public static Selector FlankTarget(AIController agent, ConstructPlayerModel model, float flankDistance, bool requireSameTeam)
        {
            return new Selector(
                new Sequence(
                    new GetModelTarget(agent, model),
                    new FlankToDestination(agent, model.modelCharacter.gameObject, flankDistance, requireSameTeam),
                    new MoveToDestination(agent, agent.distanceAllowance, 6f, true),
                    new MeleeAttack(agent, agent.currentTarget)
                    ),
                new Sequence(
                    new GetModelTarget(agent, model),
                    new FlankToDestination(agent, model.modelCharacter.gameObject, flankDistance, requireSameTeam),
                    new MoveToDestination(agent, agent.distanceAllowance, 6f, true)
                    )
                );
        }

        public static Selector IgnoreModelTargets(AIController agent, ConstructPlayerModel model)
        {
            return new Selector(
                new Sequence(
                    new GetModelNonTarget(agent, model),
                    new MoveToDestination(agent, agent.distanceAllowance, 6f, true),
                    new GetClosestEnemy(agent, agent.meleeDistance),
                    new MeleeAttack(agent, agent.currentTarget)
                    ),
                new Sequence(
                    new GetModelNonTarget(agent, model),
                    new MoveToDestination(agent, agent.distanceAllowance, 6f, true)
                    )
                );
        }

        #endregion

        #region Attacking

        public static Sequence AttackClosestTarget(AIController agent)
        {
            return new Sequence(
                new GetClosestEnemy(agent, agent.meleeDistance),
                new MoveToDestination(agent, agent.distanceAllowance, 6f, false),
                new MeleeAttack(agent, agent.currentTarget)
                );
        }

        public static Sequence MoveToTargetWhileAttacking(AIController agent, GameObject target)
        {
            return new Sequence(
                new GetClosestEnemyToTarget(agent, target),
                new MoveToDestination(agent, agent.distanceAllowance, 6f, false),
                new GetClosestEnemy(agent, agent.sightDistance),
                new MeleeAttack(agent, agent.currentTarget)
                );
        }

        #endregion
    }

    #endregion
}