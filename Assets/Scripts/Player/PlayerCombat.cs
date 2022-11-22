using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;

    int damage;
    public Transform swordBase;
    public Transform swordTip;

    public bool canMove = true;
    public bool canAttack = true;

    public void LightAttack()
    {
        if (canAttack)
        {
            Debug.Log("Light attack");
            canAttack = false;
            animator.SetTrigger("LightAttack");
            animator.SetInteger("RandAttack", Random.Range(0, animator.GetInteger("RandAttackMax") + 1));

            modelConstructor.PlayerAttack(true);
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

    public void StartAttack(int currentDamage)
    {
        damage = currentDamage;
        InvokeRepeating("AttackCheck", 0f, 0.04f);
        //Clear list of enemies hit
    }

    public void EndAttack()
    {
        CancelInvoke("AttackCheck");
        //Clear List of enemies hit
    }

    void AttackCheck()
    {
        Debug.Log("AttackCheck " + damage);
    }

    private void OnDrawGizmos()
    {
        if (swordBase != null && swordTip != null)
            Gizmos.DrawLine(swordBase.position, swordTip.position);
    }

    ConstructPlayerModel modelConstructor;

    private void Start()
    {
        modelConstructor = GameObject.FindObjectOfType<ConstructPlayerModel>();
    }
}
