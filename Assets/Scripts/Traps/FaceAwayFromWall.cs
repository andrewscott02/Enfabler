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

        if (StartRayCast(origin, Vector3.forward, 180)) return;
        if (StartRayCast(origin, Vector3.back, 0)) return;
        if (StartRayCast(origin, Vector3.left, 90)) return;
        if (StartRayCast(origin, Vector3.right, 270)) return;
    }

    bool StartRayCast(Vector3 origin, Vector3 direction, float yRot)
    {
        RaycastHit hit;
        Vector3 target = origin + (direction * range);
        Vector3 dir = target - origin;

        if (Physics.Raycast(origin, dir, out hit, range, wallMask))
        {
            transform.eulerAngles = new Vector3(0, yRot, 0);
            //Debug.Log("Hit for " + direction);
            return true;
        }

        return false;
    }
}
