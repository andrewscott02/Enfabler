using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAI : MonoBehaviour
{
    AIController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponentInParent<AIController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controller.Activate();
        }
    }
}
