using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light_ColorSpaceSet : MonoBehaviour
{
    public float gammaLightIntensity = 1f, linearLightIntensity = 2f;

    private void Start()
    {
        Light light = GetComponent<Light>();
        switch (QualitySettings.activeColorSpace)
        {
            case ColorSpace.Gamma:
                light.intensity = gammaLightIntensity;
                break;
            case ColorSpace.Linear:
                light.intensity = linearLightIntensity;
                break;
        }
    }
}
