using Enfabler.Attacking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportOnThreshold : HealthThresholdEvent
{
    public float yOffset = 1f;
    public float teleportRadius = 3f;

    public bool teleportAreaMoves = false;
    Vector3 teleportOrigin;

    protected override void Start()
    {
        base.Start();
        teleportOrigin = transform.position;
    }

    protected override void ThresholdReached()
    {
        base.ThresholdReached();

        if (HelperFunctions.GetRandomPoint(GetTeleportOrigin() - new Vector3(0, yOffset, 0), teleportRadius, 1f, 500, out Vector3 point))
        {
            //Teleport
            point.y += yOffset;
            Debug.Log("Hitbox teleports to point " + point);
            transform.position = point;
        }
        else
        {
            //Teleport
            point.y += transform.position.y;
            Debug.Log("Hitbox teleports to point (failed) " + point);
            transform.position = point;
        }
    }

    Vector3 GetTeleportOrigin()
    {
        if (teleportAreaMoves)
            return transform.position;

        return teleportOrigin;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(GetTeleportOrigin(), teleportRadius);
    }
}
