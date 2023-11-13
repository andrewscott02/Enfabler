using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RegionTextManager : MonoBehaviour
{
    public TextMeshProUGUI regionText;
    public Image regionDisplay;
    public float duration;
    public float fadeSpeed = 1.4f;

    public void ShowRegionText(string text)
    {
        Debug.Log("Show region text- region manager");
        regionText.text = text;

        //TODO: Fade in
        active = true;

        StartCoroutine(IDelayHideText(duration));
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
            Color displayColour = regionDisplay.color;
            displayColour.a = HelperFunctions.Remap(progress, 0, 1, 0, 0.4f);
            regionDisplay.color = displayColour;

            Color textColour = regionText.color;
            textColour.a = progress;
            regionText.color = textColour;
        }
    }
}
