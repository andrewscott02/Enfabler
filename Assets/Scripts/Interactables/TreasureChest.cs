using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : Interactable, IInteractable
{
    Animator animator;

    public Vector2Int goldAmount = new Vector2Int(40, 100);

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

        Vector2Int goldReward = TreasureManager.instance.GetGoldReward(goldAmount);

        int gold = Random.Range(goldReward.x, goldReward.y + 1);
        //Debug.Log("GOLD FROM CHEST: " + gold);
        TreasureManager.instance.D_GiveGold(gold);

        interactDelegate();
    }

    public delegate void InteractDelegate();
    public InteractDelegate interactDelegate;

    void InteractedDelegate()
    {
        //Blank delegate
        //Debug.Log("Interacted delegate");
    }
}
