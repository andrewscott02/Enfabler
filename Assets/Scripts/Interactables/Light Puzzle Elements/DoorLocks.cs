using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLocks : MonoBehaviour
{
    public LightReceiver[] lightReceiverUnlockers; //Replace with generic parent once functionality is complete
    public Interactable interactable;
    public bool unlockMainDoor = true;

    int locks = 0;
    int unlocked = 0;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        SetupPuzzleDoorUnlock();
    }

    public void SetupPuzzleDoorUnlock()
    {
        foreach (var item in lightReceiverUnlockers)
        {
            item.enableDelegate += item.invertUnlock ? Lock : Unlock;
            item.disableDelegate += item.invertUnlock ? Unlock : Lock;
        }

        locks = lightReceiverUnlockers.Length;
    }

    void Unlock()
    {
        Debug.Log("Unlock Interaction");
        unlocked++;

        if (unlocked >= locks)
            UnlockInteractable();
        else
            LockInteractable();
    }

    void Lock()
    {
        Debug.Log("Unlock Interaction");
        unlocked--;

        if (unlocked >= locks)
            UnlockInteractable();
        else
            LockInteractable();
    }

    void UnlockInteractable()
    {
        interactable.UnlockInteraction();
    }

    void LockInteractable()
    {
        interactable.LockInteraction();
    }
}
