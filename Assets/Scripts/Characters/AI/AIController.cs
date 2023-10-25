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
    CharacterMovement characterMovement; 

    public float roamTimeElapsed { get; protected set; } = 0;

    public override void Start()
    {
        base.Start();
        player = GameObject.FindObjectOfType<PlayerController>().gameObject;
        agent = GetComponent<NavMeshAgent>();
        characterMovement = GetComponent<CharacterMovement>();

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
        
        #region Animation and Movement Speed

        Vector3 movement = agent.velocity;
        //movement = transform.TransformDirection(movement);

        //Gets the rotation of the model to offset the animations
        Vector2 realMovement = new Vector2(0, 0);
        realMovement.x = Vector3.Dot(movement, model.right);
        realMovement.y = Vector3.Dot(movement, model.forward);

        //Sets the movement animations for the animator
        float currentSpeed = realMovement.magnitude * agent.speed * animMultiplier;
        animator.SetFloat("RunBlend", currentSpeed);

        characterMovement.currentSpeed = currentSpeed;

        #endregion

        for (int i = 0; i < attacks.Length; i++)
        {
            attacks[i].timeSinceLastAttack += Time.deltaTime;
        }

        lastParry += Time.deltaTime;
        lastDodge += Time.deltaTime;
    }
    public override void ActivateRagdoll(bool activate, ExplosiveForceData forceData)
    {
        SetDestinationPos(gameObject.transform.position);
        agent.enabled = false;
        bt.enabled = false;
        base.ActivateRagdoll(activate, forceData);
    }

    public float distanceAllowance = 1f;

    public float lerpSpeed = 0.01f;

    public LayerMask sightMask;
    public float sightDistance = 100;
    public float chaseDistance = 40;
    public float roamDistance = 25;
    public float maxDistanceFromModelCharacter = 6;

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

    public void ResetRoamTime()
    {
        //Debug.Log("Resetting roam time");
        roamTimeElapsed = 0;
    }

    public void MoveToDestination(bool sprinting)
    {
        //Debug.Log(sprinting + " sprinting");
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

    public AIAttackData[] attacks;

    [System.Serializable]
    public struct AIAttackData
    {
        public CharacterCombat.AttackType attackType;

        public float distance;
        public float attackSpeed;

        public float doubleAttackChance;
        public float unblockableChance;
        public Vector2 cooldown;
        public float attackPauseTime;

        [HideInInspector]
        public float currentCooldown;
        [HideInInspector]
        public float timeSinceLastAttack;
    }
    
    public AIAttackData GetAttackFromType(CharacterCombat.AttackType attackType)
    {
        for (int i = 0; i < attacks.Length; i++)
        {
            if (attacks[i].attackType == attackType)
            {
                return attacks[i];
            }
        }

        Debug.LogWarning("No attack type matches, returning first item");
        return attacks[0];
    }

    public bool CanAttack(CharacterCombat.AttackType attackType)
    {
        if (currentTarget == null || !combat.canAttack)
            return false;

        for (int i = 0; i < attacks.Length; i++)
        {
            if (attacks[i].attackType == attackType)
            {
                if (attacks[i].timeSinceLastAttack >= attacks[i].currentCooldown)
                {
                    //Debug.Log("Can attack for " + attackType);
                    return true;
                }
            }
        }

        //Debug.Log("Cannot attack for " + attackType);
        return false;
    }

    public bool AttackTarget(CharacterCombat.AttackType attackType)
    {
        if (currentTarget == null)
            return false;

        int attackIndex = -1;

        for (int i = 0; i < attacks.Length; i++)
        {
            if (attacks[i].attackType == attackType)
            {
                attackIndex = i;
            }
        }

        if (attackIndex < 0) return false;

        float distance = Vector3.Distance(this.gameObject.transform.position, currentTarget.gameObject.transform.position);
        //Debug.Log("Attack called + " + attackType);

        if (distance < attacks[attackIndex].distance)
        {
            if (combat.canAttack && attacks[attackIndex].timeSinceLastAttack >= attacks[attackIndex].currentCooldown)
            {
                if (doubleAttack)
                    doubleAttack = false;
                else
                    doubleAttack = Random.Range(0f, 1f) < attacks[attackIndex].doubleAttackChance;

                lastAttacked = currentTarget;
                //lastAttacked.GetCharacterCombat().StartBeingAttacked();

                //Debug.Log("Attack made");
                combat.savingChargeInput = attackType;
                combat.Attack(attacks[attackIndex].attackSpeed, true, attackType, currentTarget.gameObject);

                bool unblockable = Random.Range(0f, 1f) < attacks[attackIndex].unblockableChance;
                float releaseTime = unblockable ? combat.chargeMaxTime : attacks[attackIndex].attackPauseTime;
                StartCoroutine(IReleaseAttack(releaseTime));

                AdjustCooldowns(attackType, attackIndex);
            }

            agent.isStopped = true;
            return true;
        }

        return false;
    }

    void AdjustCooldowns(CharacterCombat.AttackType attackType, int attackIndex = -1)
    {
        if (attackIndex < 0)
        {
            for (int i = 0; i < attacks.Length; i++)
            {
                if (attacks[i].attackType == attackType)
                {
                    attackIndex = i;
                }
            }
        }

        attacks[attackIndex].timeSinceLastAttack = 0;

        if (doubleAttack)
        {
            attacks[attackIndex].currentCooldown = 0;
        }
        else
        {
            attacks[attackIndex].currentCooldown = Random.Range(attacks[attackIndex].cooldown.x, attacks[attackIndex].cooldown.y);
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

    #endregion
}
