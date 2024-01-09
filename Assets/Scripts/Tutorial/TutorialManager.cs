using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    public TextMeshProUGUI text;

    [TextArea(3, 10)]
    public string firstMessage;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        SetTutorialMessage(firstMessage);
    }

    public void SetTutorialMessage(string message)
    {
        text.text = message;
    }
}
