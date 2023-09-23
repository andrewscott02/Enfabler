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

    List<Collider> ragdollColliders = new List<Collider>();

    void SetupRagdoll()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (var item in colliders)
        {
            if (item.gameObject != this.gameObject)
            {
                item.isTrigger = this;
                ragdollColliders.Add(item);
            }
        }
    }

    public void ActivateRagdoll(bool activate)
    {
        if (activate)
        {
            Debug.Log(gameObject.name + " has activated ragdoll");
        }
        foreach (var item in ragdollColliders)
        {
            if (item.gameObject != this.gameObject)
            {
                item.isTrigger = !activate;
            }
            else
            {
                item.isTrigger = activate;
            }
        }

        animator.enabled = !activate;

        if (activate && health.dying)
        {
            combat.weapon.Disarm();
        }
    }
}
