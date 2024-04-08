using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameCanvasManager : MonoBehaviour
{
    public static GameCanvasManager instance;

    public TextMeshProUGUI text;

    private void Awake()
    {
        instance = this;
    }

    public GameObject interactObject;

    public void ShowInteractMessage(bool show, string message)
    {
        message = TextReplace.instance.ReplaceText(message);
        interactObject.SetActive(show);
        text.text = message;
    }

    public void ShowRegionText(string text)
    {
        TextPopupManager.instance.ShowRegionText(text);
    }
}
