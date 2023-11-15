using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDish : MonoBehaviour, IReceiveLight
{
    LightEmitter reflectEmitter;

    void Awake()
    {
        reflectEmitter = GetComponentInParent<LightEmitter>();
        reflectEmitter.emitting = false;
    }

    public void ReceiveLight()
    {
        //Debug.Log(gameObject.name + " is receiving light");
        reflectEmitter.EmitLight();
    }

    public void StopReceiveLight()
    {
        //Debug.Log(gameObject.name + " has stopped receiving light");
        reflectEmitter.StopEmitLight();
    }
}