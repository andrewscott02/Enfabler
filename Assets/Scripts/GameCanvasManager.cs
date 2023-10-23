using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvasManager : MonoBehaviour
{
    public static GameCanvasManager instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject interactObject;

    public void ShowInteractMessage(bool show)
    {
        interactObject.SetActive(show);
    }
}
