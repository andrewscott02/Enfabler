using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public bool invisible = false;
    public Animator animator;
    protected CharacterCombat combat;
    protected Health health;
    public Transform model;

    public bool playerTeam = true;

    public virtual void Start()
    {
        health = GetComponent<Health>();
        combat = GetComponent<CharacterCombat>();
        combat.animator = animator;

        combat.ignore.Add(health);
    }
}
