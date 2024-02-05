using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StatueTrap : MonoBehaviour
{
    public float hostileChance = 0.25f;

    CharacterCombat combat;
    Health health;

    // Start is called before the first frame update
    void Start()
    {
        combat = GetComponentInChildren<CharacterCombat>();

        Armour armour = GetComponentInChildren<Armour>();
        armour.armourSlider.gameObject.SetActive(false);
        armour.armourSlider = null;

        health = GetComponentInChildren<Health>();
        health.healthSlider.gameObject.SetActive(false);
        health.healthSlider = null;

        float chance = Random.Range(0f, 1f);

        if (chance > hostileChance)
            StartCoroutine(IDisableStatue(0.1f));
    }

    IEnumerator IDisableStatue(float delay)
    {
        yield return new WaitForSeconds(delay);

        combat.enabled = false;

        health.hitReactData.lightHitReactThreshold = 1000;
        health.hitReactData.heavyHitReactThreshold = 1000;
        health.hitReactData.killRagdoll = false;
        health.hitReactData.killDestroyTime = 0f;

        Animator animator = GetComponentInChildren<Animator>();
        animator.SetBool("StandingGuard", true);

        AIController ai = GetComponentInChildren<AIController>();
        ai.enabled = false;
        ai.bt.enabled = false;

        CharacterMovement movement = GetComponentInChildren<CharacterMovement>();
        movement.enabled = false;

        NavMeshAgent agent = GetComponentInChildren<NavMeshAgent>();
        agent.enabled = false;

        Rigidbody rb = GetComponentInChildren<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;

        yield return new WaitForSeconds(delay * 2);

        animator.speed = 0;
    }


}
