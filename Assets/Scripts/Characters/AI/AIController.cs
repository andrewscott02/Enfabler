using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTrees;
using Enfabler.Attacking;

public class AIController : BaseCharacterController
{
    #region Setup

    #region Variables

    protected GameObject player; public GameObject GetPlayer() { return player; }

    public float distanceAllowance = 1f;

    public float lerpSpeed = 0.01f;

    public LayerMask sightMask;
    private bool m_Alert = false;
    public bool alert
    {
        get
        {
            return m_Alert;
        }

        set
        {
            m_Alert = value;
            if (value == true)
                Ping();
            else
                StopPing();
        }
    }

    public bool pinged = false;
    public float pingInterval = 3f;
    public float pingResetTime = 3f;

    public bool standGuard = false;
    public float standGuardAnimSpeed = 1;

    [SerializeField]
    float passiveSightDistance = 25;
    [SerializeField]
    float alertSightDistance = 40;

    public float GetSightDistance()
    {
        //Debug.Log("Agent " + name + " is " + alert + " sight distance is " + (alert ? alertSightDistance : passiveSightDistance).ToString());
        return alert ? alertSightDistance : passiveSightDistance;
    }

    [SerializeField]
    public float passiveRoamDistance = 7;
    [SerializeField]
    float alertRoamDistance = 10;

    public float GetRoamDistance()
    {
        return alert ? alertRoamDistance : passiveRoamDistance;
    }
    
    public float maxDistanceFromModelCharacter = 6;

    public Vector3 followVector;
    public float followDistance = 5;

    #endregion

    #region Behaviour Tree
    protected NavMeshAgent agent; public NavMeshAgent GetNavMeshAgent() { return agent; }
    public BehaviourTree bt;
    CharacterMovement characterMovement; 

    public float roamTimeElapsed { get; protected set; } = 100;

    public override void Awake()
    {
        base.Awake();
        player = GameObject.FindObjectOfType<PlayerController>().gameObject;
        agent = GetComponent<NavMeshAgent>();
        characterMovement = GetComponent<CharacterMovement>();

        currentDestination = transform.position;

        ActivateAI();

        for (int i = 0; i < attacks.Length; i++)
        {
            attacks[i].usesLeft = attacks[i].maxUses <= 0 ? -1 : attacks[i].maxUses;
            if (attacks[i].healthPercentageUse <= 0)
            {
                attacks[i].healthPercentageUse = 1;
            }
        }

        for (int i = 0; i < spells.Length; i++)
        {
            spells[i].identifier = i;
            spells[i].usesLeft = spells[i].maxUses <= 0 ? -1 : spells[i].maxUses;
            if (spells[i].healthPercentageUse <= 0)
            {
                spells[i].healthPercentageUse = 1;
            }
        }

        health.HitReactionDelegate += OnHit;
        combat.dodgeEndDelegate += OnEndDodge;

        combat.blockedDelegate += BlockedAttack;
        combat.parriedDelegate += ParriedAttack;
    }

    public virtual void ActivateAI()
    {
        AIManager.instance.AllocateTeam(this);

        bt.Setup(this);
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(currentDestination, distanceAllowance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(gameObject.transform.position, alertSightDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(gameObject.transform.position, passiveSightDistance);

        if (currentTarget != null)
        {
            Vector3 origin = mainCollider.bounds.center + new Vector3(0, mainCollider.bounds.extents.y, 0);
            Vector3 target = currentTarget.mainCollider.bounds.center;
            Gizmos.DrawLine(origin, target);
        }
    }

    #endregion

    public virtual void Update()
    {
        if (health.dying) { return; }

        roamTimeElapsed += Time.deltaTime;

        if (CanRotate())
        {
            Debug.Log(name + "is rotating");
            Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
            Quaternion desiredrot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredrot, Time.deltaTime * agent.angularSpeed);
            //characterMovement.targetRotation = desiredrot;
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

        //Standing guard
        bool standingGuard = (!alert && currentSpeed == 0 && standGuard);
        animator.SetBool("StandingGuard", standingGuard);

        #endregion

        for (int i = 0; i < attacks.Length; i++)
        {
            attacks[i].timeSinceLastAttack += Time.deltaTime;
        }

        for (int i = 0; i < spells.Length; i++)
        {
            spells[i].timeSinceLastAttack += Time.deltaTime;
        }

        lastDodge += Time.deltaTime;

        hitCooldownT += Time.deltaTime;

        if (hitCooldownT >= hitTakenCooldown)
        {
            hitCooldownT = 0;
            recentHitsTaken = Mathf.Clamp(recentHitsTaken - 1, 0, forceHitResponse + 3);
            blockedAttacks = Mathf.Clamp(blockedAttacks - 1, 0, parryHitThreshold + 3);
        }
    }

    public override void ActivateRagdoll(bool activate, ExplosiveForceData forceData, bool disableAnimator = true)
    {
        SetDestinationPos(gameObject.transform.position);
        agent.enabled = !activate;
        bt.enabled = !activate;

        base.ActivateRagdoll(activate, forceData, disableAnimator);
    }

    public override void Killed()
    {
        SetDestinationPos(gameObject.transform.position);
        agent.enabled = false;
        bt.enabled = false;
        base.Killed();
    }

    bool rotationLock = false;

    bool canRotate = false;

    public bool CanRotate()
    {
        canRotate = (combat.canSaveAttackInput || characterMovement.currentSpeed > 5) && rotationLock == false && currentTarget != null;
        return canRotate;
    }

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

    public void ResetRoamTime()
    {
        //Debug.Log("Resetting roam time");
        roamTimeElapsed = 0;
    }

    public void MoveToDestination(bool sprinting)
    {
        if (!agent.enabled) return;
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

    #region Attacking

    public BaseCharacterController currentTarget;
    BaseCharacterController lastAttacked;

    bool doubleAttack;

    public bool ignoreAttackQueue = false;
    public AIAttackData[] attacks;

    [System.Serializable]
    public struct AIAttackData
    {
        public E_AttackType attackType;

        public float distance;

        public float doubleAttackChance;
        public float unblockableChance;
        public Vector2 cooldown;
        public float attackPauseTime;

        public int maxUses;
        public float healthPercentageUse;

        public bool lockMovement;

        [HideInInspector]
        public int usesLeft;
        [HideInInspector]
        public float currentCooldown;
        [HideInInspector]
        public float timeSinceLastAttack;
    }

    public AISpellData[] spells;

    [System.Serializable]
    public struct AISpellData
    {
        public SpellStats spell;
        [HideInInspector]
        public int identifier;

        public float distance;
        public Vector2 cooldown;

        public int maxUses;
        public float healthPercentageUse;

        [HideInInspector]
        public int usesLeft;
        [HideInInspector]
        public float currentCooldown;
        [HideInInspector]
        public float timeSinceLastAttack;
    }

    public int preparedAttack = -1;

    public int GetValidAttack()
    {
        if (currentTarget == null || !combat.canAttack || attacks.Length <= 0)
            return -1;

        for (int i = attacks.Length - 1; i >= 0; i--)
        {
            if (CanAttack(i))
            {
                return i;
                //TODO: could use priority to find best spell
            }
        }

        //Debug.Log("Cannot attack for " + attackType);
        return -1;
    }

    public AIAttackData GetAttackFromType(E_AttackType attackType)
    {
        for (int i = 0; i < attacks.Length; i++)
        {
            if (attacks[i].attackType == attackType)
            {
                return attacks[i];
            }
        }

        //Debug.LogWarning("No attack type matches, returning first item");
        return attacks[0];
    }

    public bool CanAttack(int attackIndex)
    {
        if (health.dying) { return false ; }

        if (currentTarget == null || !combat.canAttack)
            return false;

        if (attacks[attackIndex].timeSinceLastAttack >= attacks[attackIndex].currentCooldown && attacks[attackIndex].usesLeft != 0 && attacks[attackIndex].healthPercentageUse >= (float)health.GetCurrentHealth() / (float)health.maxHealth)
        {
            //Debug.Log("Can attack for " + attackType);
            return true;
        }

        //Debug.Log("Cannot attack for " + attackType);
        return false;
    }

    public void EnqueueAttack()
    {
        AIManager.instance.Enqueue(this);
    }

    public AISpellData GetValidSpell()
    {
        AISpellData invalidData = new AISpellData();
        if (currentTarget == null || !combat.canAttack || spells.Length <= 0)
            return invalidData;

        for (int i = 0; i < spells.Length; i++)
        {
            if (CanCastSpell(i))
            {
                return spells[i];
                //TODO: could use priority to find best spell
            }
        }

        //Debug.Log("Cannot attack for " + attackType);
        return invalidData;
    }

    public bool CanCastSpell(int identifier)
    {
        if (health.dying) { return false; }
        if (identifier < 0 || identifier >= spells.Length) return false;

        if (spells[identifier].timeSinceLastAttack >= spells[identifier].currentCooldown && spells[identifier].usesLeft != 0 && spells[identifier].healthPercentageUse >= (float)health.GetCurrentHealth() / (float)health.maxHealth)
        {
            return true;
        }
        return false;
    }

    int currentSpell;

    public void PrepareSpell(int identifier)
    {
        //Debug.Log("Preparing + " + spells[identifier].spell.spellName);
        currentSpell = identifier;
    }

    public bool CastSpell()
    {
        if (!CanCastSpell(currentSpell)) return false;

        spells[currentSpell].timeSinceLastAttack = 0;
        spells[currentSpell].currentCooldown = Random.Range(spells[currentSpell].cooldown.x, spells[currentSpell].cooldown.y);
        spells[currentSpell].usesLeft--;
        combat.CastSpell(spells[currentSpell].spell, currentTarget.gameObject);

        return true;
    }

    public bool AttackTarget(int attackIndex)
    {
        if (health.dying) { return false; }
        if (!agent.enabled) return false;

        if (currentTarget == null)
            return false;

        if (attackIndex < 0) return false;

        float distance = Vector3.Distance(this.gameObject.transform.position, currentTarget.gameObject.transform.position);
        //Debug.Log("Attack called + " + attackType);

        if (distance < attacks[attackIndex].distance)
        {
            bool unblockable = false;

            if (combat.canAttack && CanAttack(attackIndex))
            {
                if (doubleAttack)
                    doubleAttack = false;
                else
                    doubleAttack = Random.Range(0f, 1f) < attacks[attackIndex].doubleAttackChance;

                lastAttacked = currentTarget;
                //lastAttacked.GetCharacterCombat().StartBeingAttacked();

                //Debug.Log("Attack made");
                combat.savingChargeInput = attacks[attackIndex].attackType;
                combat.Attack(true, attacks[attackIndex].attackType, currentTarget.gameObject);

                unblockable = Random.Range(0f, 1f) < attacks[attackIndex].unblockableChance;
                float releaseTime = unblockable ? combat.chargeMaxTime : attacks[attackIndex].attackPauseTime;
                StartCoroutine(IReleaseAttack(attacks[attackIndex], releaseTime));

                AdjustCooldowns(attackIndex);
                attacks[attackIndex].usesLeft--;
            }

            agent.isStopped = true;

            if (!doubleAttack || unblockable)
            {
                if (AIManager.instance != null)
                {
                    AIManager.instance.Dequeue(this, true);
                }
            }

            return true;
        }

        return false;
    }

    void AdjustCooldowns(int attackIndex)
    {
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

    IEnumerator IReleaseAttack(AIAttackData attackData, float delay)
    {
        if (attackData.lockMovement)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        yield return new WaitForSeconds(delay);

        if (attackData.lockMovement)
        {
            rotationLock = true;
        }

        if (!health.dying)
        {
            combat.ReleaseAttack();
            agent.isStopped = false;
        }
    }

    public void EndAttackOnTarget()
    {
        agent.isStopped = false;
        rotationLock = false;
        agent.enabled = true;

        if (lastAttacked != null)
        {
            //lastAttacked.GetCharacterCombat().StopBeingAttacked();
            lastAttacked = null;
        }
    }

    #endregion

    #region Defensive

    #region Blocking

    public float blockChance = 0.6f;
    public float blockDuration = 2f;

    public bool CanBlock()
    {
        return (Random.Range(0f, 1f) < blockChance) && combat.canDodge;
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

    void BlockedAttack()
    {
        blockedAttacks++;

        //TODO Weapon flash to show parry is available if threshold is met
    }

    #endregion

    #region Parrying

    public bool canParry = false;
    public int parryHitThreshold = 2;
    int blockedAttacks = 0;

    public bool CanParry()
    {
        return canParry && blockedAttacks >= parryHitThreshold;
    }

    public void ActivateParry(float duration)
    {
        //Debug.Log("Activating parry");
        combat.Block(false);
        combat.Block(true, true);
        StartCoroutine(IDelayDeactivateBlock(duration));
    }

    void ParriedAttack(ICanDealDamage attacker)
    {
        blockedAttacks = 0;
    }

    #endregion

    #region Dodging

    public float dodgeChance = 0.2f;
    public float dodgeCooldown = 1f;
    float lastDodge;

    public bool CanDodge()
    {
        if (dodgeChance > 0)
        {
            if (lastDodge > dodgeCooldown)
            {
                lastDodge = 0;
                return (Random.Range(0f, 1f) < dodgeChance || health.armourScript.currentArmour == 1) && combat.canDodge;
            }
        }

        return false;
    }

    public void Dodge()
    {
        recentHitsTaken = 0;
        rotationLock = true;

        Vector3 direction = (transform.position - player.transform.position).normalized;
        Quaternion desiredrot = Quaternion.LookRotation(direction, transform.up);
        transform.rotation = desiredrot;
        characterMovement.targetRotation = desiredrot;
        //Debug.Log("Rotating " + gameObject.name + " towards direction " + direction + " with a rotation of " + desiredrot.eulerAngles + " || " + transform.rotation.eulerAngles);

        combat.Dodge();
    }

    void OnEndDodge()
    {
        rotationLock = false;
    }

    #endregion

    public int hitResponse = 2, forceHitResponse = 4;
    int recentHitsTaken = 0;
    public float hitTakenCooldown = 0.8f;
    float hitCooldownT = 0;

    void OnHit(int damage, Vector3 dir, E_AttackType attackType = E_AttackType.None)
    {
        hitCooldownT = 0;
        recentHitsTaken = Mathf.Clamp(recentHitsTaken + 1, 0, forceHitResponse + 3);
    }

    public bool CanDefend()
    {
        return recentHitsTaken > hitResponse && combat.GetTargetted() || recentHitsTaken > forceHitResponse;
    }

    #endregion

    #region Ping

    public void Ping()
    {
        if (pingCoroutine != null)
            return;

        AIManager.instance.Ping(this);
        pingCoroutine = StartCoroutine(IPingOthers());
    }

    public void StopPing()
    {
        if (pingCoroutine != null)
        {
            StopCoroutine(pingCoroutine);
            pingCoroutine = null;
        }
    }

    Coroutine pingCoroutine;

    IEnumerator IPingOthers()
    {
        yield return new WaitForSeconds(pingInterval);
        AIManager.instance.Ping(this);
    }

    public void Pinged()
    {
        pinged = true;

        if (pingedCoroutine != null)
        {
            StopCoroutine(pingedCoroutine);
            pingedCoroutine = null;
        }

        pingedCoroutine = StartCoroutine(IResetPing(pingResetTime));
    }

    Coroutine pingedCoroutine;

    IEnumerator IResetPing(float delay)
    {
        yield return new WaitForSeconds(delay);

        pinged = false;
    }

    #endregion

    #endregion
}
