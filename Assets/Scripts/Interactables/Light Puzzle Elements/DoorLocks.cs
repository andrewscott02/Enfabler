using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoorLocks : MonoBehaviour
{
    public TextMeshProUGUI text;
    public LightReceiver[] lightReceiverUnlockers; //Replace with generic parent once functionality is complete
    public Interactable interactable;
    public bool unlockMainDoor = true;

    int locks = 0;
    int unlocked = 0;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        SetupPuzzleDoorUnlock();

        SetLockUI();
    }

    public void SetupPuzzleDoorUnlock()
    {
        foreach (var item in lightReceiverUnlockers)
        {
            item.enableDelegate += item.invertUnlock ? Lock : Unlock;
            item.disableDelegate += item.invertUnlock ? Unlock : Lock;

            if (item.invertUnlock)
            {
                Unlock();
            }
        }

        locks = lightReceiverUnlockers.Length;
    }

    void Unlock()
    {
        Debug.Log("Unlock Interaction");
        unlocked++;

        SetLockUI();

        if (unlocked >= locks)
            UnlockInteractable();
        else
            LockInteractable();
    }

    void Lock()
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
