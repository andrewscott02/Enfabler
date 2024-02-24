using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPopupManager : MonoBehaviour
{
    public static TextPopupManager instance;

    public TextPopup regionText, messageText;

    void Awake()
    {
        instance = this;
    }

    public void ShowRegionText(string message)
    {
        regionText.ShowText(message);
    }

    public void ShowMessageText(string message)
    {
        messageText.ShowText(message);
    }
}
