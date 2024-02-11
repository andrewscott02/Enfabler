using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    public Vector3 rotation;

    private void Update()
    {
        Vector3 rot = transform.rotation.eulerAngles;
        rot.x += rotation.x;
        rot.y += rotation.y;
        rot.z += rotation.z;

        transform.rotation = Quaternion.Euler(rot);
    }
}
