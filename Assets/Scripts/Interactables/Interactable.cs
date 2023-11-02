using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    public static PlayerController player;

    public E_InteractTypes interactType;
    public bool lockedInteraction = false;
    public bool forceInteraction = false;
    public bool multipleInteractions = false;
    public Object interactFX;

    protected bool canBeInteracted = true;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = GameObject.FindObjectOfType<PlayerController>();
    }

    public MonoBehaviour GetScript()
    {
        return this;
    }

    public void UnlockInteraction()
    {
        lockedInteraction = false;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!canBeInteracted) return;
        if (lockedInteraction) return;

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
    }

    public void ShowInteractMessage(bool show)
    {
        GameCanvasManager.instance.ShowInteractMessage(show);
    }
}
