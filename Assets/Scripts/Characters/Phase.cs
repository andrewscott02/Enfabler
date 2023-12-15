using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phase : MonoBehaviour
{
    int defaultLayer;
    public int phaseLayer;

    // Start is called before the first frame update
    void Start()
    {
        defaultLayer = gameObject.layer;

        CharacterCombat combat = GetComponent<CharacterCombat>();
        combat.phaseDelegate += ActivatePhase;
    }

    void ActivatePhase(bool activate)
    {
        gameObject.layer = activate ? phaseLayer : defaultLayer;
    }
}
