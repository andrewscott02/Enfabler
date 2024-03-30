using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextReplace : MonoBehaviour
{
    public static TextReplace instance { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        onControlsChange += OnControlsChange;
    }

    ControllerIconReplace[] iconTooltips =
    {
        new ControllerIconReplace
        {
            replaceText = "Melee",
            imageID = 0
        },
        new ControllerIconReplace
        {
            replaceText = "Ranged",
            imageID = 1
        },
        new ControllerIconReplace
        {
            replaceText = "Dodge",
            imageID = 2
        },
        new ControllerIconReplace
        {
            replaceText = "Block",
            imageID = 3
        },
        new ControllerIconReplace
        {
            replaceText = "Interact",
            imageID = 4
        },
        new ControllerIconReplace
        {
            replaceText = "Move",
            imageID = 5
        },
        new ControllerIconReplace
        {
            replaceText = "Camera",
            imageID = 6
        }
    };

    public string ReplaceText(string text)
    {
        string newText = text;

        foreach (var icon in iconTooltips)
        {
            newText = newText.Replace("$" + icon.replaceText + "$", "<sprite index=" + icon.imageID + ">");
        }

        return newText;
    }

    public delegate void D_OnControlsChange();
    public D_OnControlsChange onControlsChange;

    void OnControlsChange()
    {
        //TODO: Change sprite sheet
    }
}

[System.Serializable]
public struct ControllerIconReplace
{
    public string title;
    public string replaceText;
    public int imageID;
}