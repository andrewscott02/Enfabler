using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    #region Setup

    [HideInInspector]
    public Animator animator;

    #endregion

    #region Basic Actions

    public void LightAttack()
    {
        if (canAttack)
        {
            Debug.Log("Light attack");
            canAttack = false;
            animator.SetTrigger("LightAttack");
            animator.SetInteger("RandAttack", Random.Range(0, animator.GetInteger("RandAttackMax") + 1));
        }
    }

    public virtual void Parry()
    {
        if (canAttack)
        {
            Debug.Log("Parry");
            canMove = false;
            //canAttack = false;
            animator.SetTrigger("Parry");

            //Remove later
            ResetAttack();
            ResetMove();
        }
    }

    public virtual void Dodge()
    {
        if (canAttack)
        {
            Debug.Log("Parry");
            canMove = false;
            //canAttack = false;
            animator.SetTrigger("Parry");

            //Remove later
            ResetAttack();
            ResetMove();
        }
    }

    #endregion

    #region Enabling/ Disabling Attacks and Movement

    public bool canMove = true;
    public bool canAttack = true;

    public void NextAttack()
    {
        Debug.Log("Next Attack");
        animator.SetInteger("SwordAttackCount", animator.GetInteger("SwordAttackCount") + 1);

        if (animator.GetInteger("SwordAttackCount") > animator.GetInteger("SwordAttackMax"))
        {
            animator.SetInteger("SwordAttackCount", 0);
        }

        canAttack = true;
    }

    public void ResetAttack()
    {
        Debug.Log("Reset Attack");
        animator.SetInteger("SwordAttackCount", 0);
        canAttack = true;
    }

    public void ResetMove()
    {
        canMove = true;
    }

    #endregion

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

    public void EndAttack()
    {
        HitEnemy(hitTargets.Count > 0);

        //Clear damage and list of enemies hit
        hitTargets.Clear();
        damage = 0;

        CancelInvoke("AttackCheck");
    }

    public virtual void HitEnemy(bool hit)
    {
        //Empty, leave for player
    }

    void AttackCheck()
    {
        Debug.Log("AttackCheck " + damage);

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
            hitHealth.Damage(damage, hit.point, hit.normal);
        }
    }

    private void OnDrawGizmos()
    {
        if (swordBase != null && swordTip != null)
            Gizmos.DrawLine(swordBase.position, swordTip.position);
    }

    #endregion
}
