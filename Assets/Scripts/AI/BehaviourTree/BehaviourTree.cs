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

        public static Selector RoamToRandomPoint(AIController agent)
        {
            return new Selector(
                DefensiveAction(agent),
                new Sequence(
                    new FindPointRadius(agent),
                    new MoveToDestination(agent, 6f, false)
                    )
                );
        }

        public static Selector MoveToClosestTarget(AIController agent)
        {
            return new Selector(
                DefensiveAction(agent),
                new Sequence(
                    new GetClosestEnemy(agent, agent.chaseDistance),
                    new MoveToDestination(agent, 6f, true)
                    )
                );
        }

        public static Selector FollowTarget(AIController agent, GameObject target, bool requireSameTeam)
        {
            return new Selector(
                DefensiveAction(agent),
                new Sequence(
                    new FindPointNearTarget(agent, target, requireSameTeam),
                    new MoveToDestination(agent, Mathf.Infinity, true)
                    )
                );
        }

        public static Selector RushToTarget(AIController agent, GameObject target)
        {
            return new Selector(
                DefensiveAction(agent),
                new Sequence(
                    new GetClosestEnemyToTarget(agent, target),
                    new MoveToDestination(agent, 6f, true),
                    new MeleeAttack(agent)
                    ),
                new Sequence(
                    new GetClosestEnemyToTarget(agent, target),
                    new MoveToDestination(agent, 6f, true)
                    )
                );
        }

        public static Selector FlankTarget(AIController agent, ConstructPlayerModel model, float flankDistance, bool requireSameTeam, bool sprint)
        {
            return new Selector(
                DefensiveAction(agent),
                new Sequence(
                    new GetModelNonTarget(agent, model),
                    new FlankToDestination(agent, model.modelCharacter.gameObject, flankDistance, requireSameTeam),
                    new MoveToDestination(agent, 6f, sprint),
                    new MeleeAttack(agent)
                    ),
                new Sequence(
                    new GetModelTarget(agent, model),
                    new FlankToDestination(agent, model.modelCharacter.gameObject, flankDistance, requireSameTeam),
                    new MoveToDestination(agent, 6f, sprint),
                    new MeleeAttack(agent)
                    ),
                new Sequence(
                    new GetModelTarget(agent, model),
                    new FlankToDestination(agent, model.modelCharacter.gameObject, flankDistance, requireSameTeam),
                    new MoveToDestination(agent, 6f, true)
                    )
                );
        }

        public static Selector InterceptTarget(AIController agent, ConstructPlayerModel model, float flankDistance, bool requireSameTeam, bool sprint)
        {
            return new Selector(
                DefensiveAction(agent),
                new Sequence(
                    new GetModelNonTarget(agent, model),
                    new InterceptTarget(agent, model.modelCharacter.gameObject, flankDistance, requireSameTeam),
                    new MoveToDestination(agent, 6f, sprint),
                    new MeleeAttack(agent)
                    ),
                new Sequence(
                    new GetModelTarget(agent, model),
                    new InterceptTarget(agent, model.modelCharacter.gameObject, flankDistance, requireSameTeam),
                    new MoveToDestination(agent, 6f, sprint),
                    new MeleeAttack(agent)
                    ),
                new Sequence(
                    new GetModelTarget(agent, model),
                    new InterceptTarget(agent, model.modelCharacter.gameObject, flankDistance, requireSameTeam),
                    new MoveToDestination(agent, 6f, true)
                    )
                );
        }

        public static Selector IgnoreModelTargets(AIController agent, ConstructPlayerModel model)
        {
            return new Selector(
                DefensiveAction(agent),
                new Sequence(
                    new GetModelNonTarget(agent, model),
                    new MoveToDestination(agent, 6f, false),
                    new GetClosestEnemy(agent, agent.meleeDistance),
                    new MeleeAttack(agent)
                    ),
                new Sequence(
                    new GetModelNonTarget(agent, model),
                    new MoveToDestination(agent, 6f, true)
                    )
                );
        }

        #endregion

        #region Combative

        public static Selector AttackClosestTarget(AIController agent)
        {
            return new Selector(
                DefensiveAction(agent),
                new Sequence(
                    new GetClosestEnemy(agent, agent.meleeDistance),
                    new MoveToDestination(agent, 6f, false),
                    new MeleeAttack(agent)
                    )
                );
        }

        public static Selector MoveToTargetWhileAttacking(AIController agent, GameObject target, float sightRadius)
        {
            return new Selector(
                DefensiveAction(agent),
                new Sequence(
                    new GetClosestEnemyToTarget(agent, target),
                    new MoveToDestination(agent, 6f, false),
                    new GetClosestEnemy(agent, agent.meleeDistance),
                    new MeleeAttack(agent)
                    )
                );
        }

        public static Sequence DefensiveAction(AIController agent)
        {
            return new Sequence(
                new BeingAttacked(agent),
                new Selector(
                    new Sequence(
                        new CanDefend(agent),
                        new Dodge(agent, agent.distanceAllowance)
                        ),
                    new Sequence(
                        new CanDefend(agent),
                        new Parry(agent)
                        )
                    )
                );
        }

        #endregion
    }

    #endregion
}