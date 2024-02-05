using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCamContainer : MonoBehaviour
{
    public static PlayerCamContainer instance;
    public CinemachineVirtualCamera exploreCam, combatCam, combatZoomCam, killCam;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        PlayerController player = GameObject.FindObjectOfType<PlayerController>();

        exploreCam.LookAt = player.gameObject.transform;
        exploreCam.Follow = player.followTarget.transform;

        combatCam.LookAt = player.gameObject.transform;
        combatCam.Follow = player.followTarget.transform;

        combatZoomCam.LookAt = player.gameObject.transform;
        combatZoomCam.Follow = player.followTarget.transform;

        killCam.LookAt = player.gameObject.transform;
        killCam.Follow = player.followTarget.transform;
    }
}
