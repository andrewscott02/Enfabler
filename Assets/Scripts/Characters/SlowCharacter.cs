using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowCharacter : MonoBehaviour
{
    CharacterCombat combatScript;
    public List<E_SlowType> slowImmunities;

    // Start is called before the first frame update
    void Start()
    {
        combatScript = GetComponent<CharacterCombat>();
    }

    public void SetAnimSpeed(float speed, E_SlowType slowType)
    {
        if (slowImmunities.Count <= 0) return;
        if (slowImmunities.Contains(slowType)) return;

        if (combatScript != null)
        {
            combatScript.SetSpeed(speed);
        }
    }

    public void ResetAnimSpeed()
    {
        if (combatScript != null)
        {
            combatScript.ResetAnimSpeed();
        }
    }
}
