using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public TrapStats trapStats;

    public void ActivateTrap()
    {
        switch (trapStats.durationType)
        {
            case E_Duration.OnlyOnce:
                ApplyEffect();
                break;
            case E_Duration.OnceDestroy:
                ApplyEffect();
                DeactivateTrap();
                Destroy(this.gameObject);
                break;
            case E_Duration.Interval:
                TrapManager.instance.TrapActivation += ApplyEffect;
                break;
            default:

                break;
        }
    }

    public void DeactivateTrap()
    {
        if (trapStats.durationType == E_Duration.Interval)
        {
            TrapManager.instance.TrapActivation -= ApplyEffect;
        }
    }

    void ApplyEffect()
    {
        if (trapStats.durationType == E_Duration.Interval)
        {
            if (TrapManager.instance.trapTime % trapStats.activateInterval != 0)
                return;
        }

        //TODO: Apply effects
        Debug.Log(gameObject.name + " was activated at" + TrapManager.instance.trapTime);
    }
}
