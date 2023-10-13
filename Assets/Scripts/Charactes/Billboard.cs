using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;
    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (cam == null) return;

        //Rotate towards active camera
        Vector3 direction = cam.transform.position - transform.position;
        Vector3 desiredRot = Quaternion.LookRotation(direction).eulerAngles;

        if (lockX) desiredRot.x = transform.rotation.x;
        if (lockY) desiredRot.y = transform.rotation.y;
        if (lockZ) desiredRot.z = transform.rotation.z;

        transform.rotation = Quaternion.Euler(desiredRot);
    }
}
