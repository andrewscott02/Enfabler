using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;

public static class HelperFunctions
{
    #region Getting Positions

    /// <summary>
    /// Gets a random point within a specified radius, attempting to use navmesh if possible
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="radius"></param>
    /// <param name="distanceAllowance"></param>
    /// <returns></returns>
    public static Vector3 GetRandomPoint(Vector3 origin, float radius, float distanceAllowance, int iterations)
    {
        Vector3 point;

        if (GetRandomPointOnNavmesh(origin, radius, distanceAllowance, iterations, out point) == false)
        {
            ///This is rarely used, but if the function cannot
            ///get a point on navmesh with the number of iterations,
            ///then generate a point ignoring navmesh as a backup
            point = GetRandomPointNonNavmesh(origin, radius);
        }

        return point;
    }

    /// <summary>
    /// Gets a random point within a specified radius, ignoring navmesh
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static Vector3 GetRandomPointNonNavmesh(Vector3 origin, float radius)
    {
        Vector3 randomPoint = origin + Random.insideUnitSphere * radius;
        randomPoint.y = origin.y;

        return randomPoint;
    }

    /// <summary>
    /// Attempts to get a random point within a specified radius within navmesh.
    /// <para />
    /// Loops, generating random points and checking if they are on navmesh
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="radius"></param>
    /// <param name="distanceAllowance"></param>
    /// <param name="iterations"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static bool GetRandomPointOnNavmesh(Vector3 origin, float radius, float distanceAllowance, int iterations, out Vector3 point)
    {
        point = new Vector3(0, 0, 0);

        //Loops based on number of iterations, generating random points and checking if they are on navmesh
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

        //If none of the points generated are on valid navmesh, return false
        return false;
    }

    /// <summary>
    /// Gets a point that positions the vector b between vector a and the return vector.
    /// <para />
    /// Can use a negative value for the distance to get a point between the 2 input vectors, close to vector b
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static Vector3 GetFlankingPoint(Vector3 a, Vector3 b, float distance)
    {
        //Get the direction from a->b and normalise it
        Vector3 direction = (b - a).normalized;

        //Increase the magnitude
        direction *= distance;

        return b + direction;
    }

    #endregion

    #region Getting Closest Enemies

    /// <summary>
    /// Gets the closest enemy to a specified origin
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="origin"></param>
    /// <param name="sightRadius"></param>
    /// <param name="debug"></param>
    /// <returns></returns>
    public static CharacterController GetClosestEnemy(AIController agent, Vector3 origin, float sightRadius, bool debug)
    {
        CharacterController closestCharacter = null;
        float closestDistance = 99999;

        foreach (var item in AIManager.instance.GetEnemyTeam(agent))
        {
            if (item == null) break;
            if (item.invisible) break;

            float itemDistance = Vector3.Distance(origin, item.gameObject.transform.position);

            if (debug)
                Debug.Log(item.gameObject.name + " is " + itemDistance);

            if (itemDistance < sightRadius && itemDistance < closestDistance)
            {
                if (debug && closestCharacter != null)
                {
                    Debug.Log("Origin: " + origin);
                    Debug.Log("From " + closestCharacter.name + closestDistance + " to " + item.name + itemDistance);
                }
                closestCharacter = item;
                closestDistance = itemDistance;
            }
        }

        return closestCharacter;
    }

    /// <summary>
    /// Gets the closest enemy to a specified origin, only considering enemies in the enemy list
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="origin"></param>
    /// <param name="sightRadius"></param>
    /// <param name="debug"></param>
    /// <param name="enemyList"></param>
    /// <returns></returns>
    public static CharacterController GetClosestEnemyFromList(Vector3 origin, float sightRadius, bool debug, List<CharacterController> enemyList)
    {
        CharacterController closestCharacter = null;
        float closestDistance = 99999;

        if (enemyList.Count == 1) { return enemyList[0]; }

        foreach (var item in enemyList)
        {
            if (item == null) break;
            if (item.invisible) break;

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

    /// <summary>
    /// Gets the closest enemy to a specified origin, ignoring enemies within the enmey list
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="origin"></param>
    /// <param name="sightRadius"></param>
    /// <param name="debug"></param>
    /// <param name="enemyList"></param>
    /// <returns></returns>
    public static CharacterController GetClosestEnemyExcludingList(AIController agent, Vector3 origin, float sightRadius, bool debug, List<CharacterController> enemyList)
    {
        CharacterController closestCharacter = null;
        float closestDistance = 99999;

        foreach (var item in AIManager.instance.GetEnemyTeam(agent))
        {
            if (item == null) break;
            if (item.invisible || enemyList.Contains(item)) break;

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

    public static Vector3 LerpVector3(Vector3 a, Vector3 b, float p)
    {
        float x = Mathf.Lerp(a.x, b.x, p);
        float y = Mathf.Lerp(a.y, b.y, p);
        float z = Mathf.Lerp(a.z, b.z, p);
        return new Vector3(x, y, z);
    }
}

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

[System.Serializable]
public struct FootStepData
{
    public Object footstepObject;
    public Transform[] footstepTransforms;
    public CinemachineImpulseSource impulseSource;
    public float impulseMultiplier;
}