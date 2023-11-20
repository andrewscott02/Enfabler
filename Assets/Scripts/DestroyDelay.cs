using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDelay : MonoBehaviour
{
    public float delay = 5f;

    // Start is called before the first frame update
    void Start()
    {
        if (delay >= 0)
        {
            Debug.Log("Setting trap duration to " + delay + " seconds");
            Destroy(this.gameObject, delay);
        }
    }
}
