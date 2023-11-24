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
        slowedCharacters = new List<SlowCharacter>();

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

        DisableSlow();

        Destroy(transform.parent.gameObject);
    }

    private void OnDestroy()
    {
        DisableSlow();
    }

    private void OnDisable()
    {
        DisableSlow();
    }

    void DisableSlow()
    {
        foreach (var item in slowedCharacters)
        {
            item.ResetAnimSpeed();
        }
    }

    #endregion

    #region Collision Events

    List<SlowCharacter> slowedCharacters;

    private void OnTriggerEnter(Collider other)
    {
        SlowCharacter slowScript = other.GetComponent<SlowCharacter>();

        if (slowScript != null)
        {
            slowedCharacters.Add(slowScript);
            slowScript.SetAnimSpeed(slowIntensity, slowType);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        SlowCharacter slowScript = other.GetComponent<SlowCharacter>();

        if (slowScript != null)
        {
            slowScript.SetAnimSpeed(slowIntensity, slowType);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        SlowCharacter slowScript = other.GetComponent<SlowCharacter>();

        if (slowScript != null)
        {
            slowedCharacters.Remove(slowScript);
            slowScript.ResetAnimSpeed();
        }
    }

    #endregion
}

public enum E_SlowType
{
    Web, Magic
}