using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#region Interfaces

public interface IDamageable
{
    void Damage(int damage, Vector3 spawnPos, Vector3 spawnRot);
    bool CheckKill();
    void Kill();
}

public interface IHealable
{
    void Heal(int heal);
}

#endregion

public static class HelperFunctions
{
    public static Vector3 GetRandomPoint(Vector3 origin, float radius, float distanceAllowance)
    {
        Vector3 point = new Vector3(0, 0, 0);

        if (GetRandomPointOnNavmesh(origin, radius, distanceAllowance, 30, out point) == false)
        {
            Debug.Log("Non navmesh");
            point = GetRandomPointNonNavmesh(origin, radius);
        }
        else
        {
            Debug.Log("Navmesh");
        }

        return point;
    }

    public static Vector3 GetRandomPointNonNavmesh(Vector3 origin, float radius)
    {
        Vector3 randomPoint = origin + Random.insideUnitSphere * radius;
        randomPoint.y = origin.y;

        return randomPoint;
    }

    public static bool GetRandomPointOnNavmesh(Vector3 origin, float radius, float distanceAllowance, int iterations, out Vector3 point)
    {
        point = new Vector3(0, 0, 0);
        for (int i = 0; i < iterations; i++)
        {
            Vector3 randomPoint = origin + Random.insideUnitSphere * radius;
            randomPoint.y = origin.y;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, distanceAllowance, NavMesh.AllAreas))
            {
                point = hit.position;
                return true;
            }
        }
        return false;
    }
}