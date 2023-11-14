using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebbingTrap : MonoBehaviour
{
    public float duration = 8;

    private void Start()
    {
        StartCoroutine(IDelayDestroy(duration));
    }

    IEnumerator IDelayDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);

        Destroy(transform.parent.gameObject);
    }

    #region Collision Events

    private void OnTriggerEnter(Collider other)
    {
        CharacterCombat combatScript = other.GetComponent<CharacterCombat>();

        if (combatScript != null)
        {
            Debug.Log(other.gameObject.name);
            combatScript.SetSpeed(0.5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterCombat combatScript = other.GetComponent<CharacterCombat>();

        if (combatScript != null)
        {
            combatScript.ResetAnimSpeed();
        }
    }

    #endregion
}