using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetAIMessage : MonoBehaviour
{
    public TextMeshProUGUI text;

    private void Start()
    {
        text.text = ExperimentManager.instance.GetAIMessage();
    }
}
