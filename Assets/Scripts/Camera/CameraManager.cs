using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    void Start()
    {
        instance = this;
    }

    bool active = false;

    [ContextMenu("Switch cam")]
    public void SwitchCam()
    {
        active = !active;
        SetCombatCam(active);
    }

    [ContextMenu("Switch cam - Kill Cam")]
    public void SwitchCamKill()
    {
        SetCombatCam(false, true);
    }

    public void SetCombatCam(bool active, bool killCam = false)
    {
        Debug.Log("Combat camera is active: " + active);
        PlayerCamContainer.instance.combatCam.Priority = active ? 11 : 9;

        if (killCam)
        {
            PlayerCamContainer.instance.killCam.Priority = 12;
            StartCoroutine(IResetBlend(2f));
        }
    }

    IEnumerator IResetBlend(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        PlayerCamContainer.instance.killCam.Priority = 8;
    }
}