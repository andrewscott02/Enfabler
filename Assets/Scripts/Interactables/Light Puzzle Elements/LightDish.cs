using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDish : MonoBehaviour, IReceiveLight
{
    public GameObject effects;

    LightEmitter reflectEmitter;

    void Awake()
    {
        reflectEmitter = GetComponentInParent<LightEmitter>();
        reflectEmitter.StopEmitLight();
    }

    public void ReceiveLight(bool harm)
    {
        //Debug.Log(gameObject.name + " is receiving light");
        reflectEmitter.EmitLight();
        if (effects != null)
            effects.SetActive(true);
    }

    public void StopReceiveLight()
    {
        //Debug.Log(gameObject.name + " has stopped receiving light");
        reflectEmitter.StopEmitLight();
        if (effects != null)
            effects.SetActive(false);
    }
}