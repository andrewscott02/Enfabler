using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : CharacterController
{
    NavMeshAgent agent;
    Vector3 currentDestination;

    public override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();

        currentDestination = transform.position;
        InvokeRepeating("NextPatrol", 0, 6f);
    }

    void NextPatrol()
    {
        currentDestination = RandPosInRadius(30);
        agent.SetDestination(currentDestination);
    }

    Vector3 RandPosInRadius(float radius)
    {
        float randX = Random.Range(0, 360);
        float randY = Random.Range(0, 360);
        float randZ = Random.Range(0, 360);
        Vector3 direction = new Vector3(randX, randY, randZ);
        direction.Normalize();

        float distance = Random.Range(0, radius);
        Vector3 point = transform.position + (direction * distance);

        return point;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(currentDestination, 1f);
    }
}
