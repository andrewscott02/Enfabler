using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_ProgressQuest : ProgressQuest
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Interactable interactable = GetComponent<Interactable>();
        interactable.onInteractDelegate += OnInteract;
    }

    public void OnInteract(BaseCharacterController interactCharacter)
    {
        ProgressQuestStage();
    }
}
