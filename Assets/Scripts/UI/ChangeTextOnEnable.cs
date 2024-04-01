using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class ChangeTextOnEnable : MonoBehaviour
{
    TextMeshProUGUI text;

    public TMP_SpriteAsset gamepad, keyboard;

    bool setup = false;
    private void Start()
    {
        PauseMenu.instance.onControlsChange += CheckText;
        usingGamepad = PlayerController.usingGamepad;

        CheckText();
    }

    void Setup()
    {
        setup = true;
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        //Debug.Log("Interact Message Enabled");
        CheckText();
    }

    bool usingGamepad = false;

    void CheckText(PlayerInput input = null)
    {
        if (!setup)
        {
            Setup();
        }
        //Debug.Log("Interact checking text");
        usingGamepad = PlayerController.usingGamepad;
        text.spriteAsset = usingGamepad ? gamepad : keyboard;

        if (TextReplace.instance != null)
            text.text = TextReplace.instance.ReplaceText(text.text);
    }
}
