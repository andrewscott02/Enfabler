using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightReceiver : MonoBehaviour, IReceiveLight
{
    public GameObject effects;

    public bool invertUnlock = false;

    public delegate void Delegate();
    public Delegate enableDelegate, disableDelegate;

    private void Awake()
    {
        enableDelegate += Enable;
        disableDelegate += Disable;
    }

    void Enable()
    {
        //Debug.Log("Enable delegate");
        effects.SetActive(true);
    }

    void Disable()
    {
        //Debug.Log("Disable delegate");
        effects.SetActive(false);
    }

    bool receivingLight = false;

    public void ReceiveLight(bool harm)
    {
        if (receivingLight)
            return;

        receivingLight = true;
        enableDelegate();
    }

    public void StopReceiveLight()
    {
        if (!receivingLight)
            return;

        receivingLight = false;
        disableDelegate();
    }
}
