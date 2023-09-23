using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    #region Setup

    protected Animator animator;
    [HideInInspector]
    public ConstructPlayerModel modelConstructor;
    protected Health health;

    public Weapon weapon { get; private set; }

    private void Start()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        ignore.Add(health);
        InvokeRepeating("CurrentTarget", 0, currentTargetCastInterval);
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

    #region Basic Actions

    BaseCharacterController[] lastAttacked;

    public void LightAttack()
    {
        if (canAttack)
        {
            Debug.Log("Attack");
            EndParry();
            EndDodge();
            ForceEndAttack();

            Target();

            canMove = false;
            canAttack = false;
            canDodge = false;
            animator.SetTrigger("LightAttack");
        }
    }

    public virtual void Block()
    {
        //Todo: Change to block
        return;
        if (canParry)
        {
            EndParry();
            EndDodge();
            ForceEndAttack();

            if (modelConstructor != null)
            {
                modelConstructor.PlayerParry(attackers > 0);
            }

            canMove = false;
            canAttack = false;
            canParry = false;
            canDodge = false;
            if (animator != null) { animator.SetTrigger("Parry"); }
        }
    }

    public virtual void Dodge()
    {
        if (canDodge)
        {
            EndParry();
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

    #endregion

    #region Enabling/ Disabling Attacks and Movement

    public bool canMove = true;
    public bool canAttack = true;
    public bool canParry = true;
    public bool canDodge = true;

    public void NextAttack()
    {
        animator.SetInteger("MeleeAttackCount", animator.GetInteger("MeleeAttackCount") + 1);

        if (animator.GetInteger("MeleeAttackCount") > animator.GetInteger("MeleeAttackMax"))
        {
            animator.SetInteger("MeleeAttackCount", 0);
        }

        canDodge = true;
        canAttack = true;
        canParry = true;

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
        canMove = true;
        canDodge = true;
        canAttack = true;
        canParry = true;

        AIController AIController = GetComponent<AIController>();

        if (AIController != null)
        {
            AIController.EndAttackOnTarget();
        }

        Untarget();
    }

    #endregion

    #region Logic

    #region Attack Logic

    List<IDamageable> hitTargets = new List<IDamageable>();
    public List<IDamageable> ignore = new List<IDamageable>();

    int damage;

    public void StartAttack(int currentDamage)
    {
        weapon.trail.SetActive(true);

        //Clear damage and list of enemies hit
        hitTargets.Clear();
        damage = currentDamage;

        InvokeRepeating("AttackCheck", 0f, 0.004f);
    }

    public void ForceEndAttack()
    {
        //Clear damage and list of enemies hit
        hitTargets.Clear();
        damage = 0;

        CancelInvoke("AttackCheck");
    }

    public void EndAttack()
    {
        weapon.trail.SetActive(false);

        HitEnemy(hitTargets.Count > 0);

        //Clear damage and list of enemies hit
        hitTargets.Clear();
        damage = 0;

        Untarget();

        CancelInvoke("AttackCheck");
    }

    public virtual void HitEnemy(bool hit)
    {
        if (modelConstructor != null)
        {
            modelConstructor.PlayerAttack(hit);
        }
    }

    public void Parried()
    {
        canAttack = false;
        canParry = false;
        animator.SetTrigger("HitReact");
        animator.SetInteger("RandReact", Random.Range(0, animator.GetInteger("RandReactMax") + 1));
    }

    public float hitSphereRadius = 0.2f;

    void AttackCheck()
    {
        //Debug.Log("AttackCheck " + damage);

        //Raycast between sword base and tip
        RaycastHit hit;

        Vector3 origin = weapon.weaponBaseHit.transform.position;
        float distance = Vector3.Distance(weapon.weaponBaseHit.transform.position, weapon.weaponTipHit.transform.position);
        Vector3 dir = weapon.weaponTipHit.transform.position - weapon.weaponBaseHit.transform.position;

        if (Physics.SphereCast(origin, radius: hitSphereRadius, direction: dir, out hit, maxDistance: distance))
        {
            IDamageable hitDamageable = hit.collider.GetComponent<IDamageable>();

            #region Guard Clauses

            //Return if collided object has no health component
            if (hitDamageable == null)
            {
                Debug.LogWarning("No interface");
                return;
            }

            //Return if it has already been hit or if it should be ignored
            if (hitTargets.Contains(hitDamageable) || ignore.Contains(hitDamageable))
            {
                Debug.LogWarning("Ignore");
                return;
            }

            #endregion

            //If it can be hit, deal damage to target and add it to the hit targets list
            hitTargets.Add(hitDamageable);
            hitDamageable.Damage(this, damage, hit.point, hit.normal);

            OnAttackHit();
        }
    }

    void OnAttackHit()
    {
        Freeze();

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

    #region Parry Logic

    protected bool parrying;

    public bool GetParrying() { return parrying; }

    public void StartParry()
    {
        parrying = true;
    }

    public void EndParry()
    {
        parrying = false;
    }

    public void ResetParry()
    {
        canDodge = true;
        ResetAttack();
    }

    #endregion

    #region Dodge Logic

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
        canParry = true;
    }

    public void ResetDodge()
    {
        canDodge = true;
        ResetAttack();
    }

    #endregion

    #region Target Logic

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
}
