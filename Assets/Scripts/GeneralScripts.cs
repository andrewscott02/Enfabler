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
    public static BaseCharacterController GetClosestEnemy(AIController agent, Vector3 origin, float sightRadius, bool debug)
    {
        BaseCharacterController closestTarget = GetClosestEnemyFromList(agent, origin, sightRadius, debug, AIManager.instance.GetEnemyTeam(agent));
        if (closestTarget != null)
            agent.ResetRoamTime();
        return closestTarget;
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
    public static BaseCharacterController GetClosestEnemyFromList(AIController agent, Vector3 origin, float sightRadius, bool debug, List<BaseCharacterController> enemyList)
    {
        BaseCharacterController closestCharacter = null;
        float closestDistance = 99999;

        foreach (var item in enemyList)
        {
            if (item == null) break;
            if (item.invisible) break;

            float itemDistance = Vector3.Distance(origin, item.gameObject.transform.position);

            CheckDistance(agent, true, sightRadius, closestDistance, closestCharacter, itemDistance, item, out closestDistance, out closestCharacter);
        }

        if (closestCharacter != null)
            agent.ResetRoamTime();
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
    public static BaseCharacterController GetClosestEnemyExcludingList(AIController agent, Vector3 origin, float sightRadius, bool debug, List<BaseCharacterController> enemyList)
    {
        BaseCharacterController closestCharacter = null;
        float closestDistance = 99999;

        foreach (var item in AIManager.instance.GetEnemyTeam(agent))
        {
            if (item == null) break;
            if (item.invisible || enemyList.Contains(item)) break;

            float itemDistance = Vector3.Distance(origin, item.gameObject.transform.position);

            CheckDistance(agent, true, sightRadius, closestDistance, closestCharacter, itemDistance, item, out closestDistance, out closestCharacter);
        }

        if (closestCharacter != null)
            agent.ResetRoamTime();

        return closestCharacter;
    }

    static void CheckDistance(AIController agent, bool requireSight, float sightRadius, float currentDistance, BaseCharacterController currentCharacter, float itemDistance, BaseCharacterController itemCharacter, out float closestDistance, out BaseCharacterController closestCharacter)
    {
        closestCharacter = currentCharacter;
        closestDistance = currentDistance;

        if ((itemDistance > sightRadius || itemDistance > closestDistance)) return;
        
        if (requireSight && !CheckSight(agent, itemCharacter, itemDistance)) return;

        closestDistance = itemDistance;
        closestCharacter = itemCharacter;
    }

    static bool CheckSight(AIController agent, BaseCharacterController targetCharacter, float sightDistance)
    {
        //Raycast between sword base and tip
        RaycastHit hit;

        Vector3 origin = agent.mainCollider.bounds.center + new Vector3(0, agent.mainCollider.bounds.extents.y, 0);
        Vector3 target = targetCharacter.mainCollider.bounds.center;
        float distance = sightDistance;
        Vector3 dir = target - origin;

        //Return if anything is blocking the agent's sight to the target
        bool canSee = !Physics.Raycast(origin, direction: dir, out hit, maxDistance: distance, agent.sightMask);

        return canSee;
    }

    #endregion

    #region Maths

    public static Vector3 GetRandomVector(Vector3 displacement)
    {
        float x = Random.Range(-displacement.x, displacement.x);
        float y = Random.Range(-displacement.y, displacement.y);
        float z = Random.Range(-displacement.z, displacement.z);

        return new Vector3(x, y, x);
    }

    public static Quaternion GetRandomQuaternion(Quaternion displacement)
    {
        float x = Random.Range(-displacement.x, displacement.x);
        float y = Random.Range(-displacement.y, displacement.y);
        float z = Random.Range(-displacement.z, displacement.z);
        float w = Random.Range(-displacement.w, displacement.w);

        return new Quaternion(x, y, z, w);
    }

    public static Vector3 LerpVector3(Vector3 a, Vector3 b, float p)
    {
        float x = Mathf.Lerp(a.x, b.x, p);
        float y = Mathf.Lerp(a.y, b.y, p);
        float z = Mathf.Lerp(a.z, b.z, p);
        return new Vector3(x, y, z);
    }

    public static float Remap(float inputValue, float fromMin, float fromMax, float toMin, float toMax)
    {
        float i = (((inputValue - fromMin) / (fromMax - fromMin)) * (toMax - toMin) + toMin);
        i = Mathf.Clamp(i, toMin, toMax);
        return i;
    }

    #endregion
}

#region Interfaces

public interface ICanDealDamage
{
    MonoBehaviour GetScript();
    E_DamageEvents DealDamage(IDamageable target, int damage, Vector3 spawnPos, Vector3 spawnRot);
    bool HitDodged();
    bool HitBlocked();
    bool HitParried();
}

public interface IDamageable
{
    MonoBehaviour GetScript();
    E_DamageEvents Damage(ICanDealDamage attacker, int damage, Vector3 spawnPos, Vector3 spawnRot);
    bool CheckKill();
    void Kill(Vector3 attacker, int damage);
    bool IsDead();
}

public interface IHealable
{
    MonoBehaviour GetScript();
    void Heal(int heal);
}

public interface IInteractable
{
    MonoBehaviour GetScript();
    void Interacted(BaseCharacterController interactCharacter);
    void ShowInteractMessage(bool show);
}

#endregion

#region Enums and Structs

public enum E_DamageEvents
{
    Hit, Block, Parry, Dodge
}

public enum E_InteractTypes
{
    Null, Button, Lever, Pray
}

[System.Serializable]
public struct FootStepData
{
    public Object footstepObject;
    public float footstepRange;
    public Transform[] footstepTransforms;
    public CinemachineImpulseSource impulseSource;
    public float impulseWalkMultiplier;
    public float impulseSprintMultiplier;
    public float impulseRange;
}


[System.Serializable]
public struct HitReactData
{
    public GameObject deathFXGO;
    public Object bloodFX, deathFX, blockFX, parryFX;

    public float hitImpulseMultiplier;
    public int lightHitReactThreshold;
    public int heavyHitReactThreshold;
    public float hitSlomoScale;
    public float hitSlomoDuration;

    public float killImpulseStrength;
    public float killSlomoScale;
    public float killSlomoDuration;
    public bool killRagdoll;
    public bool killAnim;
    public float killDestroyTime;

    public float parryImpulseStrength;
    public float parrySlomoScale;
    public float parrySlomoDuration;

    public float impulseMax;
}

public struct ExplosiveForceData
{
    public float explosiveForce;
    public Vector3 origin;

}

#endregion