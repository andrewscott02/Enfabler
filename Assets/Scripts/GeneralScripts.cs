using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#region Interfaces

public interface IDamageable
{
    void Damage(CharacterCombat attacker, int damage, Vector3 spawnPos, Vector3 spawnRot);
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
    #region Getting Positions

    public static Vector3 GetRandomPoint(Vector3 origin, float radius, float distanceAllowance)
    {
        Vector3 point = new Vector3(0, 0, 0);

        if (GetRandomPointOnNavmesh(origin, radius, distanceAllowance, 30, out point) == false)
        {
            //Debug.Log("Non navmesh");
            point = GetRandomPointNonNavmesh(origin, radius);
        }
        else
        {
            //Debug.Log("Navmesh");
        }

        return point;
    }

    public static Vector3 GetFlankingPoint(Vector3 a, Vector3 b, float distance)
    {
        //Get the direction from a->b and normalise it
        Vector3 direction = (b - a).normalized;

        //Increase the magnitude
        direction *= distance;

        return b + direction;
    }

    #region NavMesh

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

    #endregion

    #endregion

    #region Getting Closest Enemies

    public static CharacterController GetClosestEnemy(AIController agent, Vector3 origin, float sightRadius, bool debug)
    {
        CharacterController closestCharacter = null;
        float closestDistance = 99999;

        foreach (var item in AIManager.instance.GetEnemyTeam(agent))
        {
            if (item.invisible)
                break;

            float itemDistance = Vector3.Distance(origin, item.gameObject.transform.position);

            //Debug.Log(item.gameObject.name + " is " + itemDistance);

            if (itemDistance < sightRadius && itemDistance < closestDistance)
            {
                if (debug && closestCharacter != null)
                {
                    //Debug.Log("Origin: " + origin);
                    //Debug.Log("From " + closestCharacter.name + closestDistance + " to " + item.name + itemDistance);
                }
                closestCharacter = item;
                closestDistance = itemDistance;
            }
        }

        return closestCharacter;
    }

    public static CharacterController GetClosestEnemyFromList(AIController agent, Vector3 origin, float sightRadius, bool debug, List<CharacterController> enemyList)
    {
        CharacterController closestCharacter = null;
        float closestDistance = 99999;

        if (enemyList.Count == 1) { return enemyList[0]; }

        foreach (var item in enemyList)
        {
            if (item.invisible)
                break;

            float itemDistance = Vector3.Distance(origin, item.gameObject.transform.position);

            //Debug.Log(item.gameObject.name + " is " + itemDistance);

            if (itemDistance < sightRadius && itemDistance < closestDistance)
            {
                if (debug && closestCharacter != null)
                {
                    //Debug.Log("Origin: " + origin);
                    //Debug.Log("From " + closestCharacter.name + closestDistance + " to " + item.name + itemDistance);
                }
                closestCharacter = item;
                closestDistance = itemDistance;
            }
        }

        return closestCharacter;
    }

    public static CharacterController GetClosestEnemyExcludingList(AIController agent, Vector3 origin, float sightRadius, bool debug, List<CharacterController> enemyList)
    {
        CharacterController closestCharacter = null;
        float closestDistance = 99999;

        foreach (var item in AIManager.instance.GetEnemyTeam(agent))
        {
            if (item.invisible || enemyList.Contains(item))
                break;

            float itemDistance = Vector3.Distance(origin, item.gameObject.transform.position);

            //Debug.Log(item.gameObject.name + " is " + itemDistance);

            if (itemDistance < sightRadius && itemDistance < closestDistance)
            {
                if (debug && closestCharacter != null)
                {
                    //Debug.Log("Origin: " + origin);
                    //Debug.Log("From " + closestCharacter.name + closestDistance + " to " + item.name + itemDistance);
                }
                closestCharacter = item;
                closestDistance = itemDistance;
            }
        }

        return closestCharacter;
    }

    #endregion
}