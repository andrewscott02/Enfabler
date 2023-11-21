using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoorLocks : PuzzleElement
{
    public TextMeshProUGUI text;
    public Interactable interactable;
    public bool unlockMainDoor = true;

    int locks = 0;
    int unlocked = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        SetLockUI();
    }

    public override void SetupPuzzleInteractions()
    {
        foreach (var item in lightReceiverUnlockers)
        {
            item.receiver.enableDelegate += item.invertInteraction ? Deactivate : Activate;
            item.receiver.disableDelegate += item.invertInteraction ? Activate : Deactivate;

            if (item.invertInteraction)
            {
                Activate();
            }
        }

        locks = lightReceiverUnlockers.Length;
    }

    protected override void Activate()
    {
        //Debug.Log("Unlock Interaction");
        unlocked++;

        SetLockUI();

        if (unlocked >= locks)
            UnlockInteractable();
        else
            LockInteractable();
    }

    protected override void Deactivate()
    {
        Debug.Log("Unlock Interaction");
        unlocked--;

        SetLockUI();

        if (unlocked >= locks)
            UnlockInteractable();
        else
            LockInteractable();
    }

    void SetLockUI()
    {
        text.text = (locks - unlocked).ToString();
    }

    void UnlockInteractable()
    {
        if (interactable != null)
            interactable.UnlockInteraction();
    }

    void LockInteractable()
    {
        if (interactable != null)
            interactable.LockInteraction();
    }
}
