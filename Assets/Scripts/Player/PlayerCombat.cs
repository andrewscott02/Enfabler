using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    #region Setup

    [HideInInspector]
    public Animator animator;

    int damage;
    public Transform swordBase;
    public Transform swordTip;

    public bool canMove = true;
    public bool canAttack = true;

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

            modelConstructor.PlayerAttack(false);
        }
    }

    public void Parry()
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

            modelConstructor.PlayerParry(true);
        }
    }

    public void Dodge()
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

            modelConstructor.PlayerDodge(true, true);
        }
    }

    #endregion

    #region Enabling/ Disabling Attacks and Movement

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

    public void StartAttack(int currentDamage)
    {
        hitTargets.Clear();
        damage = currentDamage;
        InvokeRepeating("AttackCheck", 0f, 0.04f);
        //Clear list of enemies hit
    }

    public void EndAttack()
    {
        HitEnemy(hitTargets.Count > 0);
        hitTargets.Clear();
        CancelInvoke("AttackCheck");
        //Clear List of enemies hit
    }

    public void HitEnemy(bool hit)
    {
        modelConstructor.PlayerAttack(hit);
    }

    void AttackCheck()
    {
        Debug.Log("AttackCheck " + damage);

        //Raycast between sword base and tip
        //Upon hitting a character with a health component, check if it has already been hit or if it should be ignored
        //If it can be hit, deal damage to target and add it to the hit targets list
    }

    private void OnDrawGizmos()
    {
        if (swordBase != null && swordTip != null)
            Gizmos.DrawLine(swordBase.position, swordTip.position);
    }

    #endregion

    #region Buddy AI

    ConstructPlayerModel modelConstructor;

    private void Start()
    {
        modelConstructor = GameObject.FindObjectOfType<ConstructPlayerModel>();
    }

    #endregion
}