using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public CinemachineVirtualCamera exploreCam, combatCam;

    void Start()
    {
        instance = this;
    }

    public void SetCombatCam(bool active)
    {
        Debug.Log("Combat camera is active: " + active);
        combatCam.Priority = active ? 11 : 9;
    }
}