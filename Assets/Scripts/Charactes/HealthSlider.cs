using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    public Slider slider;

    public void ChangeSliderValue(int newValue, int newMax)
    {
        slider.maxValue = newMax;
        slider.value = newValue;
    }
}