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
        PlayerCamContainer.instance.exploreCam.Priority = active ? 8 : 11;

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

    [ContextMenu("Combat Zoom")]
    public void CombatZoom()
    {
        PlayerCamContainer.instance.combatZoomCam.Priority = 10;
        StartCoroutine(IResetCombatCam(0.2f));
    }

    IEnumerator IResetCombatCam(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        PlayerCamContainer.instance.combatZoomCam.Priority = 7;
    }
}