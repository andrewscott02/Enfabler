using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyInteract : Interactable, IInteractable
{
    public override void Interacted(BaseCharacterController interactCharacter)
    {
        base.Interacted(interactCharacter);

        PauseMenu.instance.ShowDifficultyMenu(true);
    }
}
