using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendorInteract : Interactable, IInteractable
{
    public override void Interacted(BaseCharacterController interactCharacter)
    {
        base.Interacted(interactCharacter);

        VendorManager.instance.OpenVendorMenu(true);
    }
}
