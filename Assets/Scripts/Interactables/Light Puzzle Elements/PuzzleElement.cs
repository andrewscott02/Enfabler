using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleElement : MonoBehaviour
{
    public PuzzleElements[] lightReceiverUnlockers; //Replace with generic parent once functionality is complete

    // Start is called before the first frame update
    protected virtual void Start()
    {
        SetupPuzzleInteractions();
    }

    public virtual void SetupPuzzleInteractions()
    {
        foreach (var item in lightReceiverUnlockers)
        {
            item.receiver.enableDelegate += item.invertInteraction ? Deactivate : Activate;
            item.receiver.disableDelegate += item.invertInteraction ? Activate : Deactivate;
        }
    }

    protected virtual void Activate()
    {
        Debug.Log("Unlock Interaction");
    }

    protected virtual void Deactivate()
    {
        Debug.Log("Unlock Interaction");
    }
}

[System.Serializable]
public struct PuzzleElements
{
    public LightReceiver receiver;
    public bool invertInteraction;
}
