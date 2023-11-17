using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowArea : MonoBehaviour
{
    #region Setup

    public E_SlowType slowType;
    public float slowIntensity = 0.6f;
    public float duration = 8;
    public LayerMask groundLayer;

    private void Start()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, maxDistance: 5f, layerMask: groundLayer))
        {
            transform.position = hit.point;
        }

        if (duration > 0)
            StartCoroutine(IDelayDestroy(duration));
    }

    IEnumerator IDelayDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);

        Destroy(transform.parent.gameObject);
    }

    #endregion

    #region Collision Events

    private void OnTriggerStay(Collider other)
    {
        SlowCharacter slowScript = other.GetComponent<SlowCharacter>();

        if (slowScript != null)
        {
            Debug.Log(other.gameObject.name);
            slowScript.SetAnimSpeed(slowIntensity, slowType);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SlowCharacter slowScript = other.GetComponent<SlowCharacter>();

        if (slowScript != null)
        {
            slowScript.ResetAnimSpeed();
        }
    }

    #endregion
}

public enum E_SlowType
{
    Web, Magic
}