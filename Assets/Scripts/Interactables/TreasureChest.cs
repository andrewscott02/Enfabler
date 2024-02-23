using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : Interactable, IInteractable
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
        animator.SetTrigger("OpenChest");
        //TODO: Add gold
        interactDelegate();
    }

    public delegate void InteractDelegate();
    public InteractDelegate interactDelegate;

    void InteractedDelegate()
    {
        //Blank delegate
        Debug.Log("Interacted delegate");
    }
}
