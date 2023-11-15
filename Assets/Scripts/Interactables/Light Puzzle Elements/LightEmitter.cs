using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightEmitter : MonoBehaviour, IEmitLight
{
    public bool emitOnStart = true;
    public bool canEmit = true;
    bool emitting = false;
    public bool canHarm = false;

    public LayerMask hitObjects;
    public GameObject rayObject;
    public float rayRadius = 2f;
    public float maxDistance = 100;

    IReceiveLight lastLightReceiver;

    void Start()
    {
        StopEmitLight();

        if (emitOnStart)
        {
            StartCoroutine(IDelayEmitStart(1f));
        }
    }

    IEnumerator IDelayEmitStart(float delay)
    {
        yield return new WaitForSeconds(delay);

        EmitLight();
    }

    // Update is called once per frame
    void Update()
    {
        rayObject.SetActive(emitting);
        if (!emitting) return;

        RaycastHit rayHit;
        float distance = maxDistance;

        if (Physics.SphereCast(rayObject.transform.position, rayRadius, transform.forward, out rayHit, maxDistance, hitObjects))
        {
            distance = Vector3.Distance(rayObject.transform.position, rayHit.point);

            IReceiveLight lightReceiver = rayHit.collider.GetComponent<IReceiveLight>();

            if (lastLightReceiver != null && lightReceiver != null && lightReceiver != lastLightReceiver)
                lastLightReceiver.StopReceiveLight();

            if (lightReceiver != null)
            {
                lightReceiver.ReceiveLight(canHarm);
                lastLightReceiver = lightReceiver;
            }
        }
        else
        {
            if (lastLightReceiver != null)
                lastLightReceiver.StopReceiveLight();

            lastLightReceiver = null;
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
        if (!canEmit) return;
        if (emitting) return;
        //Debug.Log(gameObject.name + " is emitting light");
        emitting = true;
    }

    public void StopEmitLight()
    {
        if (!emitting) return;

        //Debug.Log(gameObject.name + " has stopped emitting light");
        emitting = false;
        if (lastLightReceiver != null)
            lastLightReceiver.StopReceiveLight();
        lastLightReceiver = null;
    }
}
