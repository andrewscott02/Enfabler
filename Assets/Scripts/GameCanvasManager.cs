using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvasManager : MonoBehaviour
{
    public static GameCanvasManager instance;

    private void Awake()
    {
        instance = this;
        regionText = GetComponentInChildren<RegionTextManager>();
    }

    public GameObject interactObject, nextLevelUI, defeatUI;

    public void ShowInteractMessage(bool show)
    {
        interactObject.SetActive(show);
    }

    RegionTextManager regionText;

    public void ShowRegionText(string text)
    {
        regionText.ShowRegionText(text);
    }
}
