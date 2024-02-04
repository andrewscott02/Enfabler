using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    public CinemachineVirtualCamera exploreCam, combatCam;
    CinemachineBrain cmBrain;

    public float killCamBlend = 0.15f;

    void Start()
    {
        instance = this;

        cmBrain = Camera.main.GetComponent<CinemachineBrain>();
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
        combatCam.Priority = active ? 11 : 9;

        if (killCam)
        {
            cmBrain.m_CustomBlends.m_CustomBlends[0].m_Blend.m_Time = killCamBlend;
            StartCoroutine(IResetBlend(2f));
        }
    }

    IEnumerator IResetBlend(float delay)
    {
        yield return new WaitForSeconds(delay);

        cmBrain.m_CustomBlends.m_CustomBlends[0].m_Blend.m_Time = cmBrain.m_DefaultBlend.m_Time;
    }
}