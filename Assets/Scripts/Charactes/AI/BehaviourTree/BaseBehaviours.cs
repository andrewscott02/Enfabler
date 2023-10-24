using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTrees
{
    public static class BaseBehaviours
    {
        #region Movement

        public static Selector RoamToRandomPoint(AIController agent)
        {
            return new Selector(
                DefensiveAction(agent),
                new Sequence(
                    new FindPointRadius(agent, agent.roamDistance),
                    new MoveToDestination(agent, 6f, false, agent.distanceAllowance)
                    )
                );
        }

        public static Selector MoveToClosestTarget(AIController agent, float distance, bool sprinting)
        {
            return new Selector(
                DefensiveAction(agent),
                new Sequence(
                    new GetClosestEnemy(agent, agent.chaseDistance),
                    new MoveToDestination(agent, 6f, sprinting, distance)
                    )
                );
        }

        public static Selector MoveToRange(AIController agent, float radius, bool sprinting)
        {
            return new Selector(
                DefensiveAction(agent),
                new Sequence(
                    new GetClosestEnemy(agent, agent.chaseDistance),
                    new FindPointOuterRadius(agent, radius),
                    new MoveToDestination(agent, 6f, sprinting, agent.distanceAllowance)
                    )
                );
        }

        public static Selector FollowTarget(AIController agent, GameObject target, bool requireSameTeam)
        {
            return new Selector(
                DefensiveAction(agent),
                new Sequence(
                    new FindPointNearTarget(agent, target, requireSameTeam),
                    new MoveToDestination(agent, Mathf.Infinity, true, agent.distanceAllowance)
                    )
                );
        }

        public static Selector RushToTarget(AIController agent, GameObject target)
        {
            return new Selector(
                DefensiveAction(agent),
                new Sequence(
                    new GetClosestEnemyToTarget(agent, target),
                    new MoveToDestination(agent, 6f, true, agent.distanceAllowance),
                    new Attack(agent, CharacterCombat.AttackType.PrimaryAttack)
                    ),
                new Sequence(
                    new GetClosestEnemyToTarget(agent, target),
                    new MoveToDestination(agent, 6f, true, agent.distanceAllowance)
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
                    new MoveToDestination(agent, 6f, sprint, agent.distanceAllowance),
                    new Attack(agent, CharacterCombat.AttackType.PrimaryAttack)
                    ),
                new Sequence(
                    new GetModelTarget(agent, model),
                    new FlankToDestination(agent, model.modelCharacter.gameObject, flankDistance, requireSameTeam),
                    new MoveToDestination(agent, 6f, sprint, agent.distanceAllowance),
                    new Attack(agent, CharacterCombat.AttackType.PrimaryAttack)
                    ),
                new Sequence(
                    new GetModelTarget(agent, model),
                    new FlankToDestination(agent, model.modelCharacter.gameObject, flankDistance, requireSameTeam),
                    new MoveToDestination(agent, 6f, true, agent.distanceAllowance)
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
                    new MoveToDestination(agent, 6f, sprint, agent.distanceAllowance),
                    new Attack(agent, CharacterCombat.AttackType.PrimaryAttack)
                    ),
                new Sequence(
                    new GetModelTarget(agent, model),
                    new InterceptTarget(agent, model.modelCharacter.gameObject, flankDistance, requireSameTeam),
                    new MoveToDestination(agent, 6f, sprint, agent.distanceAllowance),
                    new Attack(agent, CharacterCombat.AttackType.PrimaryAttack)
                    ),
                new Sequence(
                    new GetModelTarget(agent, model),
                    new InterceptTarget(agent, model.modelCharacter.gameObject, flankDistance, requireSameTeam),
                    new MoveToDestination(agent, 6f, true, agent.distanceAllowance)
                    )
                );
        }

        public static Selector IgnoreModelTargets(AIController agent, ConstructPlayerModel model)
        {
            return new Selector(
                DefensiveAction(agent),
                new Sequence(
                    new GetModelNonTarget(agent, model),
                    new MoveToDestination(agent, 6f, false, agent.distanceAllowance),
                    new GetClosestEnemy(agent, agent.meleeDistance),
                    new Attack(agent, CharacterCombat.AttackType.PrimaryAttack)
                    ),
                new Sequence(
                    new GetModelNonTarget(agent, model),
                    new MoveToDestination(agent, 6f, true, agent.distanceAllowance)
                    )
                );
        }

        #endregion

        #region Combative

        public static Selector AttackClosestTarget(AIController agent, bool sprinting, float range, CharacterCombat.AttackType attackType = CharacterCombat.AttackType.PrimaryAttack)
        {
            return new Selector(
                DefensiveAction(agent),
                new Sequence(
                    new GetClosestEnemy(agent, range),
                    new MoveToDestination(agent, 6f, sprinting, agent.distanceAllowance),
                    new Attack(agent, attackType)
                    )
                );
        }

        public static Selector MoveToTargetWhileAttacking(AIController agent, GameObject target)
        {
            return new Selector(
                DefensiveAction(agent),
                new Sequence(
                    new GetClosestEnemyToTarget(agent, target),
                    new MoveToDestination(agent, 6f, false, agent.distanceAllowance),
                    new GetClosestEnemy(agent, agent.meleeDistance),
                    new Attack(agent, CharacterCombat.AttackType.PrimaryAttack)
                    )
                );
        }

        public static Sequence DefensiveAction(AIController agent)
        {
            return new Sequence(
                new BeingAttacked(agent),
                new Selector(
                    new Sequence(
                        new CanDodge(agent),
                        new Dodge(agent, agent.distanceAllowance)
                        ),
                    new Sequence(
                        new CanParry(agent),
                        new Parry(agent)
                        )
                    )
                );
        }

        #endregion
    }
}
