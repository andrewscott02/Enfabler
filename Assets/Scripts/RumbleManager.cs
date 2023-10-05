using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleManager : MonoBehaviour
{
    public static RumbleManager instance;

    float defaultTimeScale = 1;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    Gamepad gamePad;

    public void ControllerRumble(float lowFreq, float highFreq, float duration)
    {
        gamePad = Gamepad.current;

        if (gamePad == null)
            return;

        gamePad.SetMotorSpeeds(lowFreq, highFreq);
        StartCoroutine(IResetRumble(duration));
    }

    IEnumerator IResetRumble(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        gamePad.SetMotorSpeeds(0, 0);
    }

    private void OnDisable()
    {
        if (gamePad != null)
            gamePad.SetMotorSpeeds(0, 0);
    }
}
