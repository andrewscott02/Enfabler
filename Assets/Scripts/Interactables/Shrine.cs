using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : MonoBehaviour, IInteractable
{
    public static PlayerController player;

    public E_InteractTypes interactType;
    public bool multipleInteractions = false;
    public int healing = 30;
    public Object interactFX;

    bool canBeInteracted = true;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerController>();
    }

    public MonoBehaviour GetScript()
    {
        return this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canBeInteracted) return;

        if (other.gameObject == player.gameObject)
        {
            ShowInteractMessage(true);
            player.EnableInteraction(interactType, this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!canBeInteracted) return;

        if (other.gameObject == player.gameObject)
        {
            ShowInteractMessage(false);
            player.EnableInteraction(E_InteractTypes.Null);
        }
    }

    public void Interacted(BaseCharacterController interactCharacter)
    {
        if (!canBeInteracted) return;

        if (!multipleInteractions)
            canBeInteracted = false;

        ShowInteractMessage(false);
        player.EnableInteraction(E_InteractTypes.Null);

        interactCharacter.GetHealth().Heal(30);
        Instantiate(interactFX, interactCharacter.transform);
    }

    public void ShowInteractMessage(bool show)
    {
        GameCanvasManager.instance.ShowInteractMessage(show);
    }
}
