using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceAwayFromWall : MonoBehaviour
{
    public LayerMask wallMask;

    public float startHeight = 1f;
    public float range = 5f;

    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position + new Vector3(0, startHeight, 0);
        Gizmos.DrawSphere(origin, 0.15f);
        Gizmos.DrawLine(origin, origin + (Vector3.forward * range));
    }

    private void Start()
    {
        Vector3 origin = transform.position + new Vector3(0, startHeight, 0);

        if (Physics.Linecast(origin, origin + (Vector3.forward * range), wallMask))
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            return;
        }

        if (Physics.Linecast(origin, origin + (Vector3.left * range), wallMask))
        {
            transform.eulerAngles = new Vector3(0, 90, 0);
            return;
        }

        if (Physics.Linecast(origin, origin + (Vector3.right * range), wallMask))
        {
            transform.eulerAngles = new Vector3(0, 270, 0);
            return;
        }
    }
}
