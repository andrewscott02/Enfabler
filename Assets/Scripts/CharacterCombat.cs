using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    #region Setup

    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public ConstructPlayerModel modelConstructor;

    private void Start()
    {
        InvokeRepeating("CurrentTarget", 0, currentTargetCastInterval);
    }

    public void SetupAllies(List<CharacterController> allies)
    {
        foreach (var item in allies)
        {
            ignore.Add(item.GetHealth());
        }
    }

    #endregion

    #region Basic Actions

    CharacterController[] lastAttacked;

    public void LightAttack()
    {
        if (canAttack)
        {
            EndParry();
            EndDodge();
            ForceEndAttack();

            Target();

            canAttack = false;
            canDodge = false;
            animator.SetTrigger("LightAttack");
            //animator.SetInteger("RandAttack", 1);
            animator.SetInteger("RandAttack", Random.Range(0, animator.GetInteger("RandAttackMax") + 1));
        }
    }

    public virtual void Parry()
    {
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
        if (canAttack && canDodge)
        {
            EndParry();
            EndDodge();
            ForceEndAttack();

            if (modelConstructor != null)
            {
                modelConstructor.PlayerDodge(attackers > 0);
            }

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
        //Debug.Log("Next Attack");
        animator.SetInteger("SwordAttackCount", animator.GetInteger("SwordAttackCount") + 1);

        if (animator.GetInteger("SwordAttackCount") > animator.GetInteger("SwordAttackMax"))
        {
            animator.SetInteger("SwordAttackCount", 0);
        }

        canAttack = true;

        AIController AIController = GetComponent<AIController>();

        if (AIController != null)
        {
            AIController.EndAttackOnTarget();
        }
    }

    public void ResetAttack()
    {
        //Debug.Log("Reset Attack");
        animator.SetInteger("SwordAttackCount", 0);
        canAttack = true;
        canParry = true;

        AIController AIController = GetComponent<AIController>();

        if (AIController != null)
        {
            AIController.EndAttackOnTarget();
        }

        Untarget();
    }

    public void ResetMove()
    {
        canMove = true;
    }

    #endregion

    #region Logic

    #region Attack Logic

    List<Health> hitTargets = new List<Health>();
    public List<Health> ignore;

    int damage;
    public Transform swordBase;
    public Transform swordTip;

    public void StartAttack(int currentDamage)
    {
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

    void AttackCheck()
    {
        //Debug.Log("AttackCheck " + damage);

        //Raycast between sword base and tip
        RaycastHit hit;

        if (Physics.Linecast(swordBase.transform.position, swordTip.transform.position, out hit))
        {
            Health hitHealth = hit.collider.GetComponent<Health>();

            #region Guard Clauses

            //Return if collided object has no health component
            if (hitHealth == null)
                return;

            //Return if it has already been hit or if it should be ignored
            if (hitTargets.Contains(hitHealth) || ignore.Contains(hitHealth))
                return;

            #endregion

            //If it can be hit, deal damage to target and add it to the hit targets list
            hitTargets.Add(hitHealth);
            hitHealth.Damage(this, damage, hit.point, hit.normal);
        }
    }

    private void OnDrawGizmos()
    {
        if (swordBase != null && swordTip != null)
            Gizmos.DrawLine(swordBase.position, swordTip.position);

        RaycastHit[] hit = Physics.SphereCastAll(transform.position, currentTargetCastRadius, transform.forward, currentTargetCastDistance, layerMask);
        foreach (RaycastHit item in hit) { Gizmos.DrawWireSphere(item.point, 1f); }
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
        ResetMove();
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
    }

    public void ResetDodge()
    {
        canDodge = true;
        ResetAttack();
        ResetMove();
    }

    #endregion

    #region Target Logic

    #region Targetting

    [Header("Targeting")]
    public List<CharacterController> currentTargets;
    List<CharacterController> lastHit = new List<CharacterController>();
    public LayerMask layerMask;
    public float currentTargetCastInterval = 0.6f;
    public float currentTargetCastRadius = 1.5f;
    public float currentTargetCastDistance = 10;

    void CurrentTarget()
    {
        List<CharacterController> hitCharacters = new List<CharacterController>();

        RaycastHit[] hit = Physics.SphereCastAll(transform.position, currentTargetCastRadius, transform.forward, currentTargetCastDistance, layerMask);
        foreach (RaycastHit item in hit)
        {
            Debug.Log("Ray hit " + item.collider.gameObject.name);
            CharacterController character = item.collider.transform.gameObject.GetComponent<CharacterController>();

            if (character != null)
            {
                if (AIManager.instance.OnSameTeam(GetComponent<CharacterController>(), character) == false)
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
