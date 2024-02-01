using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObject : MonoBehaviour
{
    public GameObject enableObject;

    private void Start()
    {
        Invoke("Disable", 0.6f);
    }

    private void OnDisable()
    {
        if (!show)
        enableObject.SetActive(false);
    }

    bool show = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enableObject.SetActive(true);
            show = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enableObject.SetActive(false);
            show = false;
        }
    }
}