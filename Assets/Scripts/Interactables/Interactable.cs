using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    public static PlayerController player;

    public E_InteractTypes interactType;
    public string interactMessage = "Press O to Interact";

    public bool lockedInteraction = false;
    public bool interactOnUnlock = false;

    public bool forceInteraction = false;
    public bool multipleInteractions = false;
    public Object interactFX;

    public LightReceiver lightReceiverUnlocker; //Replace with generic parent once functionality is complete
    public bool invertUnlock = false;

    protected bool canBeInteracted = true;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = GameObject.FindObjectOfType<PlayerController>();

        SetupPuzzleDoorUnlock();

        onInteractDelegate += OnInteract;
    }

    public void SetupPuzzleDoorUnlock()
    {
        if (lightReceiverUnlocker != null)
        {
            lightReceiverUnlocker.enableDelegate += invertUnlock ? LockInteraction : UnlockInteraction;
            lightReceiverUnlocker.disableDelegate += invertUnlock ? UnlockInteraction : LockInteraction;
        }
    }

    public MonoBehaviour GetScript()
    {
        return this;
    }

    public void UnlockInteraction()
    {
        Debug.Log("Unlock Interaction");
        lockedInteraction = false;

        if (interactOnUnlock)
            Interacted(null);
    }

    public void LockInteraction()
    {
        Debug.Log("Unlock Interaction");
        lockedInteraction = true;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!canBeInteracted) return;
        if (lockedInteraction) return;
        if (player == null) return;

        if (other.gameObject == player.gameObject)
        {
            if (forceInteraction)
            {
                Interacted(player);
            }
            else
            {
                ShowInteractMessage(true);
                player.EnableInteraction(interactType, this);
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!canBeInteracted) return;
        if (lockedInteraction) return;

        if (other.gameObject == player.gameObject)
        {
            if (!forceInteraction)
            {
                ShowInteractMessage(false);
                player.EnableInteraction(E_InteractTypes.Null);
            }
        }
    }

    public delegate void OnInteractedDelegate(BaseCharacterController interactCharacter);
    public OnInteractedDelegate onInteractDelegate;

    public virtual void Interacted(BaseCharacterController interactCharacter)
    {
        if (!canBeInteracted) return;
        if (lockedInteraction) return;

        if (!multipleInteractions)
        {
            canBeInteracted = false;
            if (!forceInteraction)
            {
                ShowInteractMessage(false);
                player.EnableInteraction(E_InteractTypes.Null);
            }
        }

        if (interactFX != null)
            Instantiate(interactFX, transform);

        onInteractDelegate(interactCharacter);
    }

    public void OnInteract(BaseCharacterController interactCharacter)
    {
        //Empty delegate
    }

    public void ShowInteractMessage(bool show)
    {
        GameCanvasManager.instance.ShowInteractMessage(show, interactMessage);
    }
}
