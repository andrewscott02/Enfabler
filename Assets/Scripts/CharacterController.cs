using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public bool invisible = false;
    public Animator animator;
    protected CharacterCombat combat; public CharacterCombat GetCharacterCombat() { return combat; }
    protected Health health; public Health GetHealth() { return health; }
    public Transform model;

    public bool playerTeam = true;

    public virtual void Start()
    {
        health = GetComponent<Health>();
        health.animator = animator;
        combat = GetComponent<CharacterCombat>();
        combat.animator = animator;
    }
}
