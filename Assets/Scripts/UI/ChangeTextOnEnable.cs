using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class ChangeTextOnEnable : MonoBehaviour
{
    TextMeshProUGUI text;
    string originalText;

    public TMP_SpriteAsset gamepad, keyboard;

    bool setup = false;
    private void Setup()
    {
        setup = true;
        text = GetComponent<TextMeshProUGUI>();
        originalText = text.text;

        PauseMenu.instance.onControlsChange += CheckText;
        usingGamepad = PlayerController.usingGamepad;

        CheckText();
    }

    private void OnEnable()
    {
        if (!setup)
            Setup();

        Debug.Log("Interact Message Enabled");
        CheckText();
    }

    bool usingGamepad = false;

    void CheckText(PlayerInput input = null)
    {
        Debug.Log("Interact checking text");
        usingGamepad = PlayerController.usingGamepad;
        text.spriteAsset = usingGamepad ? gamepad : keyboard;

        text.text = TextReplace.instance.ReplaceText(originalText);
    }
}
