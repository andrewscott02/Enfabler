using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReflectLight : MonoBehaviour
{
    public LightEmitter lightEmitter;

    private void Start()
    {
        CharacterCombat combat = GetComponentInParent<CharacterCombat>();
        combat.blockingDelegate += CanReflectLight;

        CanReflectLight(false);
    }

    public void CanReflectLight(bool canReflect)
    {
        lightEmitter.canEmit = canReflect;
        if (!canReflect)
            lightEmitter.StopEmitLight();
    }
}
