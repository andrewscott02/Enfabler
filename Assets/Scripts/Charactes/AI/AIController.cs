using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTrees;

public class AIController : BaseCharacterController
{
    #region Setup

    protected GameObject player; public GameObject GetPlayer() { return player; }

    #region Behaviour Tree
    protected NavMeshAgent agent; public NavMeshAgent GetNavMeshAgent() { return agent; }
    public BehaviourTree bt;

    public float roamTimeElapsed { get; protected set; } = 0;

    public override void Start()
    {
        base.Start();
        player = GameObject.FindObjectOfType<PlayerController>().gameObject;
        agent = GetComponent<NavMeshAgent>();

        currentDestination = transform.position;

        ActivateAI();
    }

    public virtual void ActivateAI()
    {
        AIManager.instance.AllocateTeam(this);

        bt.Setup(this);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(currentDestination, distanceAllowance);

        Gizmos.DrawWireSphere(gameObject.transform.position, chaseDistance);
        Gizmos.DrawWireSphere(gameObject.transform.position, roamDistance);
        Gizmos.DrawWireSphere(gameObject.transform.position, maxDistanceFromModelCharacter);
        Gizmos.DrawWireSphere(gameObject.transform.position, meleeDistance);
    }

    private void OnDrawGizmos()
    {
        if (currentTarget != null)
        {
            Vector3 origin = mainCollider.bounds.center + new Vector3(0, mainCollider.bounds.extents.y, 0);
            Vector3 target = currentTarget.mainCollider.bounds.center;
            Gizmos.DrawLine(origin, target);
        }
    }

    public virtual void Update()
    {
        if (health.dying) { return; }

        roamTimeElapsed += Time.deltaTime;

        if (combat.canSaveAttackInput)
        {
            Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
            Quaternion desiredrot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredrot, Time.deltaTime * agent.angularSpeed);
        }
        
        #region Animation

        Vector3 movement = agent.velocity;
        //movement = transform.TransformDirection(movement);

        //Gets the rotation of the model to offset the animations
        Vector2 realMovement = new Vector2(0, 0);
        realMovement.x = Vector3.Dot(movement, model.right);
        realMovement.y = Vector3.Dot(movement, model.forward);

        //Sets the movement animations for the animator
        float currentSpeed = realMovement.magnitude * agent.speed * animMultiplier;
        animator.SetFloat("RunBlend", currentSpeed);

        #endregion

        if (currentMeleeCooldown > timeSinceLastMeleeAttack)
        {
            timeSinceLastMeleeAttack += Time.deltaTime;
        }

        if (currentRangedCooldown > timeSinceLastRangedAttack)
        {
            timeSinceLastRangedAttack += Time.deltaTime;
        }

        lastParry += Time.deltaTime;
        lastDodge += Time.deltaTime;
    }

    public float distanceAllowance = 1f;

    public float lerpSpeed = 0.01f;

    public LayerMask sightMask;
    public float sightDistance = 100;
    public float chaseDistance = 40;
    public float roamDistance = 25;
    public float maxDistanceFromModelCharacter = 6;
    public float meleeDistance = 3;
    public float rangedDistance = 15f;
    public float meleeAttackSpeed = 0.75f;

    public Vector3 followVector;
    public float followDistance = 5;

    #endregion

    #region Behaviours

    #region Movement

    public float walkSpeed = 6;
    public float sprintSpeed = 10;
    public float animMultiplier = 1f;

    public Vector3 currentDestination { get; protected set; }
    public void SetDestinationPos(Vector3 pos)
    {
        currentDestination = pos;
    }

    public bool roaming = false;

    public void MoveToDestination(bool sprinting)
    {
        agent.speed = sprinting ? sprintSpeed : walkSpeed;
        agent.SetDestination(currentDestination);
        //Rotate(currentDestination);
    }

    public bool NearDestination(float distanceAllowance)
    {
        return Vector3.Distance(transform.position, currentDestination) <= distanceAllowance;
    }

    #endregion

    #region Combat

    public BaseCharacterController currentTarget;
    BaseCharacterController lastAttacked;

    bool doubleAttack;
    public float doubleAttackChance;
    public float unblockableChance = 0.2f;
    public Vector2 meleeCooldown, rangedCooldown;
    float currentMeleeCooldown, currentRangedCooldown;
    public float attackPauseTime = 0.8f;
    float timeSinceLastMeleeAttack, timeSinceLastRangedAttack;

    public bool CanAttack(CharacterCombat.AttackType attackType)
    {
        if (currentTarget == null)
            return false;

        float currentCooldown = currentMeleeCooldown;
        float timeSinceLastAttack = timeSinceLastMeleeAttack;

        switch (attackType)
        {
            case CharacterCombat.AttackType.PrimaryAttack:
                currentCooldown = currentMeleeCooldown;
                timeSinceLastAttack = timeSinceLastMeleeAttack;
                break;
            case CharacterCombat.AttackType.SecondaryAttack:
                currentCooldown = currentRangedCooldown;
                timeSinceLastAttack = timeSinceLastRangedAttack;
                break;
            default:
                break;
        }

        if (combat.canAttack && timeSinceLastAttack >= currentCooldown)
        {
            //Debug.Log("Can attack for " + attackType);
            return true;
        }

        //Debug.Log("Cannot attack for " + attackType);
        return false;
    }

    public bool AttackTarget(CharacterCombat.AttackType attackType)
    {
        if (currentTarget == null)
            return false;

        float distance = Vector3.Distance(this.gameObject.transform.position, currentTarget.gameObject.transform.position);
        //Debug.Log("Attack called + " + attackType);

        Vector2 attackCooldown = meleeCooldown;
        float attackRange = meleeDistance;
        float currentCooldown = currentMeleeCooldown;
        float timeSinceLastAttack = timeSinceLastMeleeAttack;

        switch (attackType)
        {
            case CharacterCombat.AttackType.SecondaryAttack:
                attackCooldown = rangedCooldown;
                attackRange = rangedDistance;
                currentCooldown = currentRangedCooldown;
                timeSinceLastAttack = timeSinceLastRangedAttack;
                break;
            default:
                break;
        }

        if (distance < attackRange)
        {
            if (combat.canAttack && timeSinceLastAttack >= currentCooldown)
            {
                if (doubleAttack)
                    doubleAttack = false;
                else
                    doubleAttack = Random.Range(0f, 1f) < doubleAttackChance;

                lastAttacked = currentTarget;
                //lastAttacked.GetCharacterCombat().StartBeingAttacked();

                //Debug.Log("Attack made");
                combat.savingChargeInput = attackType;
                combat.Attack(meleeAttackSpeed, true, attackType);

                bool unblockable = Random.Range(0f, 1f) < unblockableChance;
                float releaseTime = unblockable ? combat.chargeMaxTime : attackPauseTime;
                StartCoroutine(IReleaseAttack(releaseTime));

                AdjustCooldowns(attackType);
            }

            agent.isStopped = true;
            return true;
        }

        return false;
    }

    void AdjustCooldowns(CharacterCombat.AttackType attackType)
    {
        switch (attackType)
        {
            case CharacterCombat.AttackType.PrimaryAttack:
                timeSinceLastMeleeAttack = 0;

                if (doubleAttack)
                {
                    currentMeleeCooldown = 0;
                }
                else
                {
                    currentMeleeCooldown = Random.Range(meleeCooldown.x, meleeCooldown.y);
                }
                break;
            case CharacterCombat.AttackType.SecondaryAttack:
                timeSinceLastRangedAttack = 0;

                if (doubleAttack)
                {
                    currentRangedCooldown = 0;
                }
                else
                {
                    currentRangedCooldown = Random.Range(rangedCooldown.x, rangedCooldown.y);
                }
                break;
            default:
                break;
        }
    }

    IEnumerator IReleaseAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!health.dying)
        {
            combat.ReleaseAttack(CharacterCombat.AttackType.PrimaryAttack);
            agent.isStopped = false;
        }
    }

    public void EndAttackOnTarget()
    {
        if (lastAttacked != null)
        {
            //lastAttacked.GetCharacterCombat().StopBeingAttacked();
            lastAttacked = null;
        }
    }

    public float parryChance = 0f;
    public float parryCooldown = 1f;
    float lastParry;

    public bool CanParry()
    {
        if (lastParry > parryCooldown)
        {
            lastParry = 0;
            return (Random.Range(0f, 1f) < parryChance) && combat.canDodge;
        }

        return false;
    }

    public void ActivateBlock(float duration)
    {
        combat.Block(true, false);
        StartCoroutine(IDelayDeactivateBlock(duration));
    }

    IEnumerator IDelayDeactivateBlock(float delay)
    {
        yield return new WaitForSeconds(delay);
        combat.Block(false, false);
    }

    public float dodgeChance = 0f;
    public float dodgeCooldown = 1f;
    float lastDodge;

    public bool CanDodge()
    {
        if (lastDodge > dodgeCooldown)
        {
            lastDodge = 0;
            return (Random.Range(0f, 1f) < parryChance) && combat.canDodge;
        }

        return false;
    }

    #endregion

    public void ResetRoamTime()
    {
        //Debug.Log("Resetting roam time");
        roamTimeElapsed = 0;
    }

    public override void ActivateRagdoll(bool activate, ExplosiveForceData forceData)
    {
        SetDestinationPos(gameObject.transform.position);
        agent.enabled = false;
        bt.enabled = false;
        base.ActivateRagdoll(activate, forceData);
    }

    #endregion
}
