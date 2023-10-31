using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable, IInteractable
{
    Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    public override void Interacted(BaseCharacterController interactCharacter)
    {
        base.Interacted(interactCharacter);

        animator.SetTrigger("OpenDoor");
    }
}