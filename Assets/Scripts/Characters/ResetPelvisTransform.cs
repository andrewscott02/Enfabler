using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPelvisTransform : MonoBehaviour
{
    Vector3 pos = new Vector3(0, 0, 0);
    Vector3 rot = new Vector3(-90, 0, 90);
    Vector3 scale = new Vector3(1, 1, 1);

    void Awake()
    {
        pos = transform.localPosition;
        rot = transform.rotation.eulerAngles;
        scale = transform.localScale;

        Invoke("ResetTransform", 1f);
        Invoke("ResetTransform", 5f);
    }

    void ResetTransform()
    {
        transform.localPosition = pos;
        transform.rotation = Quaternion.Euler(rot);
        transform.localScale = scale;
    }
}
