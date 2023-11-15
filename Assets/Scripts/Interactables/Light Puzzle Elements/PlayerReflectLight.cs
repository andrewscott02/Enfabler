using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReflectLight : MonoBehaviour
{
    public GameObject lightReceiverGO;

    private void Start()
    {
        CharacterCombat combat = GetComponentInParent<CharacterCombat>();
        combat.blockingDelegate += CanReflectLight;

        lightReceiverGO.SetActive(false);
    }

    public void CanReflectLight(bool canReceiveLight)
    {
        lightReceiverGO.SetActive(canReceiveLight);
    }
}
