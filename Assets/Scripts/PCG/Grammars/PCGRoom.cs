using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCGRoom : MonoBehaviour
{
    public Transform exitPoint;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue - new Color(0, 0, 0, 0.5f);
        Gizmos.DrawCube(transform.position + new Vector3(0, 2f, 0), new Vector3(2, 3, 1));

        Gizmos.color = Color.red - new Color(0, 0, 0, 0.5f);
        Gizmos.DrawCube(exitPoint.position + new Vector3(0, 2f, 0), new Vector3(2, 3, 1));
    }
}
