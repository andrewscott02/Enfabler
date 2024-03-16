using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMasterInteract : Interactable, IInteractable
{
    public override void Interacted(BaseCharacterController interactCharacter)
    {
        base.Interacted(interactCharacter);

        PauseMenu.instance.ShowDungeonMasterMenu(true);
    }
}
