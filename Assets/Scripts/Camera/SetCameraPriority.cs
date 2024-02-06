using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SetCameraPriority : MonoBehaviour
{
    public CinemachineVirtualCamera cvCam;
    public int enabledPriority = 14, disabledPriority = 6;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Collide with " + other.name);

            cvCam.Priority = enabledPriority;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Stay with " + other.name);
            if (cvCam.Priority == enabledPriority) return;
            cvCam.Priority = enabledPriority;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Stopped colliding with " + other.name);

            cvCam.Priority = disabledPriority;
        }
    }
}
