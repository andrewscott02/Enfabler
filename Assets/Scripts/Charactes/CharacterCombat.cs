using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombat : MonoBehaviour, ICanDealDamage
{
    #region Setup

    protected Animator animator;
    [HideInInspector]
    public ConstructPlayerModel modelConstructor;
    protected Health health;

    public Weapon weapon { get; private set; }

    public bool canMove = true;
    public bool sprinting = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        ignore.Add(health);
        InvokeRepeating("CurrentTarget", 0, currentTargetCastInterval);
        currentArmour = maxArmour;
        armourRegenCoroutine = StartCoroutine(IResetArmour(armourRegen));
    }

    public MonoBehaviour GetScript()
    {
        return this;
    }

    public void SetupAllies(List<BaseCharacterController> allies)
    {
        foreach (var item in allies)
        {
            ignore.Add(item.GetHealth());
        }
    }

    public void SetupWeapon(Weapon weapon)
    {
        this.weapon = weapon;
    }

    #endregion

    #region Attacking

    BaseCharacterController[] lastAttacked;
    public bool canAttack = true;

    #region Attacking -> Attack Inputs

    public void LightAttack()
    {
        if (canAttack)
        {
            //Debug.Log("Attack " + animator.GetInteger("MeleeAttackCount") + (sprinting?" Sprint":" Standard"));
            EndDodge();
            ForceEndAttack();

            Target();

            blocking = false;
            canMove = false;
            canAttack = false;
            canDodge = false;
            animator.SetTrigger(sprinting ? "SprintAttack" : "LightAttack");
            //RumbleManager.instance.ControllerRumble(0.25f, 1f, 0.25f);
        }
    }

    public void NextAttack(int attack)
    {
        //Debug.Log("Next attack + " + attack);
        animator.SetInteger("MeleeAttackCount", attack);

        canDodge = true;
        canAttack = true;

        AIController AIController = GetComponent<AIController>();

        if (AIController != null)
        {
            AIController.EndAttackOnTarget();
        }
    }

    public void ResetAttack()
    {
        Debug.Log("Reset Attack");
        if (animator == null)
            Debug.LogWarning("Animator of " + gameObject.name + " is null");
        animator.SetInteger("MeleeAttackCount", 0);
        animator.SetBool("InHitReaction", false);
        canMove = true;
        canDodge = true;
        canAttack = true;

        AIController AIController = GetComponent<AIController>();

        if (AIController != null)
        {
            AIController.EndAttackOnTarget();
        }

        Untarget();
    }

    #endregion

    #region Attacking -> Hit Logic

    List<IDamageable> hitTargets = new List<IDamageable>();
    public List<IDamageable> ignore = new List<IDamageable>();

    int damage;

    public void StartAttack(int currentDamage)
    {
        if (weapon != null)
        {
            if (weapon.weaponTrail != null)
                weapon.weaponTrail.SetActive(true);

        }

        //Clear damage and list of enemies hit
        hitTargets.Clear();
        damage = currentDamage;

        InvokeRepeating("AttackCheck", 0f, 0.004f);
    }

    public void ForceEndAttack()
    {
        //Clear damage and list of enemies hit
        if (weapon != null)
        {
            if (weapon.weaponTrail != null)
                weapon.weaponTrail.SetActive(false);

            if (weapon.bloodTrail != null)
                weapon.bloodTrail.SetActive(false);

        }

        hitTargets.Clear();
        damage = 0;

        CancelInvoke("AttackCheck");
    }

    public void EndAttack()
    {
        HitEnemy(hitTargets.Count > 0);
        Untarget();
        ForceEndAttack();
    }

    public virtual void HitEnemy(bool hit)
    {
        if (modelConstructor != null)
        {
            modelConstructor.PlayerAttack(hit);
        }
    }

    public bool HitDodged()
    {
        return true;
    }

    public bool HitBlocked()
    {
        return true;
    }

    public bool HitParried()
    {
        canAttack = false;
        animator.SetTrigger("HitReactLight");
        return true;
    }

    public float hitSphereRadius = 0.2f;

    void AttackCheck()
    {
        if (weapon == null)
            return;

        //Raycast between sword base and tip
        RaycastHit hit;

        Vector3 origin = weapon.weaponBaseHit.transform.position;
        float distance = Vector3.Distance(weapon.weaponBaseHit.transform.position, weapon.weaponTipHit.transform.position);
        Vector3 dir = weapon.weaponTipHit.transform.position - weapon.weaponBaseHit.transform.position;

        if (Physics.SphereCast(origin, radius: hitSphereRadius, direction: dir, out hit, maxDistance: distance, layerMask))
        {
            IDamageable hitDamageable = hit.collider.GetComponent<IDamageable>();

            if (hitDamageable == null)
            {
                hitDamageable = hit.collider.GetComponentInParent<IDamageable>();
            }

            #region Guard Clauses

            //Return if collided object has no health component
            if (hitDamageable == null)
            {
                Debug.LogWarning("No interface");
                return;
            }

            //Return if it has already been hit or if it should be ignored
            if (hitTargets.Contains(hitDamageable) || ignore.Contains(hitDamageable) || hitDamageable.IsDead())
            {
                Debug.LogWarning("Ignore");
                return;
            }

            #endregion

            //If it can be hit, deal damage to target and add it to the hit targets list
            hitTargets.Add(hitDamageable);
            DealDamage(hitDamageable, damage, hit.point, hit.normal);

            OnAttackHit();
        }
    }

    public E_DamageEvents DealDamage(IDamageable target, int damage, Vector3 spawnPos, Vector3 spawnRot)
    {
        return target.Damage(this, damage, spawnPos, spawnRot);
    }

    void OnAttackHit()
    {
        Freeze();
        RumbleManager.instance.ControllerRumble(0.2f, 0.85f, 0.25f);
        weapon.bloodTrail.SetActive(true);
        //TODO: Sound effects
    }

    public float hitFreezeTime = 0.15f;

    void Freeze()
    {
        if (animator == null) return;

        StartCoroutine(IFreeze(hitFreezeTime));
    }

    IEnumerator IFreeze(float delay)
    {
        Debug.Log("Freeze");
        animator.speed = 0;
        yield return new WaitForSecondsRealtime(delay);
        Debug.Log("Unfreeze");
        animator.speed = 1;
    }

    #endregion

    #region Attacking -> Target Logic

    #region Targetting

    [Header("Targeting")]
    public List<BaseCharacterController> currentTargets;
    List<BaseCharacterController> lastHit = new List<BaseCharacterController>();
    public LayerMask layerMask;
    public float currentTargetCastInterval = 0.6f;
    public float currentTargetCastRadius = 1.5f;
    public float currentTargetCastDistance = 10;

    void CurrentTarget()
    {
        List<BaseCharacterController> hitCharacters = new List<BaseCharacterController>();

        RaycastHit[] hit = Physics.SphereCastAll(transform.position, currentTargetCastRadius, transform.forward, currentTargetCastDistance, layerMask);
        foreach (RaycastHit item in hit)
        {
            //Debug.Log("Ray hit " + item.collider.gameObject.name);
            BaseCharacterController character = item.collider.transform.gameObject.GetComponent<BaseCharacterController>();

            if (character != null)
            {
                if (AIManager.instance == null)
                {
                    hitCharacters.Add(character);
                }
                else if (AIManager.instance.OnSameTeam(GetComponent<BaseCharacterController>(), character) == false)
                    hitCharacters.Add(character);
            }
        }

        currentTargets = hitCharacters;
    }

    void Target()
    {
        lastHit.Clear();
        foreach (var item in currentTargets)
        {
            item.GetCharacterCombat().StartBeingAttacked();
            lastHit.Add(item);
        }
    }

    void Untarget()
    {
        foreach (var item in lastHit)
        {
            item.GetCharacterCombat().StopBeingAttacked();
        }

        lastHit.Clear();
    }

    #endregion

    #region Targetted

    int attackers = 0; public bool GetTargetted() { return attackers > 0; }

    public void StartBeingAttacked()
    {
        attackers++;
    }

    public void StopBeingAttacked()
    {
        attackers--;
    }

    #endregion

    #endregion

    #endregion

    #region Blocking

    public int maxArmour = 5;
    public int currentArmour { get; private set; }
    public float armourCooldown = 6f;
    public float armourRegen = 3f;

    Coroutine armourRegenCoroutine;

    public virtual void Block(bool blocking)
    {
        if (this.blocking == blocking)
            return;

        Debug.Log("Block changed");
        EndDodge();
        ForceEndAttack();
        this.parrying = blocking;
        this.blocking = blocking;

        if (modelConstructor != null)
        {
            modelConstructor.PlayerParry(attackers > 0);
        }

        animator.SetInteger("MeleeAttackCount", 0);

        if (animator != null) { animator.SetBool("Blocking", blocking); }
    }

    public void ConsumeArmour()
    {
        if (armourRegenCoroutine != null)
            StopCoroutine(armourRegenCoroutine);

        currentArmour--;
        armourRegenCoroutine = StartCoroutine(IResetArmour(armourCooldown));
        ChangeArmourUI();
    }

    public void GainArmour(int armour)
    {
        currentArmour = Mathf.Clamp(currentArmour + armour, 0, maxArmour);
        ChangeArmourUI();
    }

    public bool CanBlock()
    {
        return blocking && currentArmour > 0;
    }

    IEnumerator IResetArmour(float delay)
    {
        yield return new WaitForSeconds(delay);
        GainArmour(1);
        armourRegenCoroutine = StartCoroutine(IResetArmour(armourRegen));
    }

    void ChangeArmourUI()
    {
        //TODO update UI
    }

    #endregion

    #region Parrying

    public bool blocking { get; protected set; }
    public bool parrying { get; protected set; }

    public void EndParryWindow()
    {
        parrying = false;
    }

    #endregion

    #region Dodging

    public bool canDodge = true;

    public virtual void Dodge()
    {
        if (canDodge)
        {
            Block(false);
            EndDodge();
            ForceEndAttack();

            if (modelConstructor != null)
            {
                modelConstructor.PlayerDodge(attackers > 0);
            }

            canMove = false;
            canAttack = false;
            canDodge = false;
            if (animator != null) { animator.SetTrigger("Dodge"); }
        }
    }

    #region Dodge -> Logic

    protected bool dodging;

    public bool GetDodging() { return dodging; }

    public void StartDodge()
    {
        dodging = true;
    }

    public void EndDodge()
    {
        dodging = false;
        canMove = true;
        canDodge = true;
        canAttack = true;
    }

    public void ResetDodge()
    {
        canDodge = true;
        ResetAttack();
    }

    #endregion

    #endregion

    #region Taking Damage

    public bool rumbleOnHit = false;

    public void GotHit()
    {
        Debug.Log("Got hit, end attack");
        canAttack = false;
        if (rumbleOnHit)
            RumbleManager.instance.ControllerRumble(0.25f, 1f, 0.25f);
    }

    #endregion
}