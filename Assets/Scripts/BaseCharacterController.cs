using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour
{
    public bool invisible = false;
    protected Animator animator;
    protected CharacterCombat combat; public CharacterCombat GetCharacterCombat() { return combat; }
    protected Health health; public Health GetHealth() { return health; }
    public Transform model;

    public bool playerTeam = true;

    protected SetWeapon setWeapon;

    public virtual void Start()
    {
        animator = GetComponent<Animator>();

        health = GetComponent<Health>();
        health.animator = animator;
        combat = GetComponent<CharacterCombat>();

        setWeapon = GetComponentInChildren<SetWeapon>();
        combat.SetupWeapon(setWeapon.CreateWeapon());

        SetupRagdoll();
    }

    Collider mainCollider;
    List<Collider> ragdollColliders = new List<Collider>();

    void SetupRagdoll()
    {
        mainCollider = GetComponent<Collider>();
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (var item in colliders)
        {
            if (item != mainCollider)
            {
                Rigidbody rbItem = item.GetComponent<Rigidbody>();
                rbItem.useGravity = false;

                item.isTrigger = this;
                ragdollColliders.Add(item);
            }
        }
    }

    public void ActivateRagdoll(bool activate)
    {
        foreach (var item in ragdollColliders)
        {
            if (item != mainCollider)
            {
                Rigidbody rbItem = item.GetComponent<Rigidbody>();
                rbItem.useGravity = activate;

                item.isTrigger = !activate;
            }
        }

        mainCollider.isTrigger = activate;
        mainCollider.enabled = !activate;

        animator.enabled = !activate;

        if (activate && health.dying)
        {
            combat.weapon.Disarm();
        }
    }
}
