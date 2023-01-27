using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] strikes;

    private void Update()
    {
        #region Movement and Camera

        if (Input.GetAxisRaw("Horizontal") >= 0.5 || Input.GetAxisRaw("Vertical") >= 0.5)
        {
            strikes[0].SetActive(true);
        }

        if (Input.GetAxisRaw("Horizontal") <= -0.5 || Input.GetAxisRaw("Vertical") <= -0.5)
        {
            strikes[0].SetActive(true);
        }

        if (Input.GetButtonDown("Walk"))
        {
            strikes[1].SetActive(true);
        }

        if (Input.GetAxisRaw("Mouse X") == 0.5 || Input.GetAxisRaw("Mouse Y") == 0.5)
        {
            strikes[2].SetActive(true);
        }

        #endregion

        #region Actions

        if (Input.GetButtonDown("Light Attack"))
        {
            strikes[3].SetActive(true);
        }

        if (Input.GetButtonDown("Parry"))
        {
            strikes[4].SetActive(true);
        }

        if (Input.GetButtonDown("Dodge"))
        {
            strikes[5].SetActive(true);
        }

        #endregion

        bool allDone = true;

        foreach (var strike in strikes)
        {
            if (strike.activeSelf == false)
            {
                allDone = false;
            }
        }

        if (allDone)
        {
            Debug.Log("Tutorial complete");
            //end tutorial
        }
    }
}
