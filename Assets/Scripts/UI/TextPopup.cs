using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextPopup : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public Image messageDisplay;
    public float duration;
    public float fadeSpeed = 1.4f;

    public void ShowText(string text)
    {
        textMesh.text = text;

        //TODO: Fade in
        active = true;

        StopAllCoroutines();
        if (active)
            StartCoroutine(IDelayHideText(duration));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator IDelayHideText(float delay)
    {
        yield return new WaitForSeconds(delay);

        //TODO: Fade out
        active = false;
    }

    bool active = false;
    float progress = 0;

    private void Update()
    {
        bool changed = false;

        if (active)
        {
            if (progress < 1)
            {
                progress = Mathf.Clamp(progress + (Time.deltaTime * fadeSpeed), 0, 1);

                changed = true;
            }
        }
        else
        {
            if (progress > 0)
            {
                progress = Mathf.Clamp(progress - (Time.deltaTime * fadeSpeed), 0, 1);

                changed = true;
            }
        }

        if (changed)
        {
            Color displayColour = messageDisplay.color;
            displayColour.a = HelperFunctions.Remap(progress, 0, 1, 0, 0.4f);
            messageDisplay.color = displayColour;

            Color textColour = textMesh.color;
            textColour.a = progress;
            textMesh.color = textColour;
        }
    }
}
