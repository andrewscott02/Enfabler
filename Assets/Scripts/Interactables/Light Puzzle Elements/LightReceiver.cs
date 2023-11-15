using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightReceiver : MonoBehaviour, IReceiveLight
{
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
        //Empty delegate function
        Debug.Log("Enable delegate");
    }

    void Disable()
    {
        //Empty delegate function
        Debug.Log("Disable delegate");
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
