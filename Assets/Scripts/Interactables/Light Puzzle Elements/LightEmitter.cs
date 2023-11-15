using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEmitter : MonoBehaviour, IEmitLight
{
    public bool emitting = true;
    public bool canHarm = false;

    public LayerMask hitObjects;
    public GameObject rayObject;
    public float rayRadius = 2f;
    public float maxDistance = 100;

    IReceiveLight lastLightReceiver;

    // Update is called once per frame
    void Update()
    {
        rayObject.SetActive(emitting);
        if (!emitting) return;

        RaycastHit rayHit;
        float distance = maxDistance;
        IReceiveLight lightReceiver = null;

        if (Physics.SphereCast(rayObject.transform.position, rayRadius, transform.forward, out rayHit, maxDistance, hitObjects))
        {
            distance = Vector3.Distance(rayObject.transform.position, rayHit.point);

            lightReceiver = rayHit.collider.GetComponent<IReceiveLight>();
        }

        if (lastLightReceiver != null && lightReceiver == null && lightReceiver != lastLightReceiver)
            lastLightReceiver.StopReceiveLight();

        if (lightReceiver != null)
        {
            lightReceiver.ReceiveLight(canHarm);
            lastLightReceiver = lightReceiver;
        }

        Vector3 scale = rayObject.transform.localScale;
        scale.z = distance / 2;
        rayObject.transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(rayObject.transform.position, rayRadius);
        Gizmos.DrawWireSphere(rayObject.transform.position + (transform.forward * maxDistance), rayRadius);
    }

    public void EmitLight()
    {
        //Debug.Log(gameObject.name + " is emitting light");
        emitting = true;
    }

    public void StopEmitLight()
    {
        //Debug.Log(gameObject.name + " has stopped emitting light");
        emitting = false;
        if (lastLightReceiver != null)
            lastLightReceiver.StopReceiveLight();
    }
}
