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
        interactDelegate += InteractedDelegate;
    }

    public override void Interacted(BaseCharacterController interactCharacter)
    {
        base.Interacted(interactCharacter);
        animator.SetTrigger("OpenDoor");
        interactDelegate();
    }

    public void CloseDoor()
    {
        canBeInteracted = true;
        if (animator != null)
            animator.SetTrigger("CloseDoor");
    }

    public delegate void InteractDelegate();
    public InteractDelegate interactDelegate;

    void InteractedDelegate()
    {
        //Blank delegate
        Debug.Log("Interacted delegate");
    }
}