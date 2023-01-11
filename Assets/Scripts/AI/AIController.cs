using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTrees;

public class AIController : CharacterController
{
    #region Setup

    bool active = false;

    protected GameObject player;
    protected NavMeshAgent agent; public NavMeshAgent GetNavMeshAgent() { return agent; }
    public BehaviourTree bt;

    public GameObject destinationTest;
    protected Vector3 currentDestination; public Vector3 GetDestination() { return currentDestination; }
    public void SetDestinationPos(Vector3 pos)
    {
        currentDestination = pos;
    }
    public bool roaming = false;
    public void MoveToDestination()
    {
        agent.SetDestination(currentDestination);
    }
    public bool NearDestination(float distanceAllowance)
    {
        return Vector3.Distance(transform.position, currentDestination) <= distanceAllowance;
    }

    protected CharacterController currentTarget;

    public override void Start()
    {
        base.Start();
        player = GameObject.FindObjectOfType<PlayerController>().gameObject;
        agent = GetComponent<NavMeshAgent>();

        currentDestination = transform.position;

        ActivateAI();
    }

    public virtual void ActivateAI()
    {
        AIManager.instance.AllocateTeam(this);

        bt.Setup(this);
    }

    #endregion

    protected void NextPatrol()
    {
        currentDestination = RandPosInRadius(transform.position, 30);
        agent.SetDestination(currentDestination);
    }

    protected Vector3 RandPosInRadius(Vector3 origin, float radius)
    {
        float randX = Random.Range(0, 360);
        float randY = Random.Range(0, 360);
        float randZ = Random.Range(0, 360);
        Vector3 direction = new Vector3(randX, randY, randZ);
        direction.Normalize();

        float distance = Random.Range(0, radius);
        Vector3 point = origin + (direction * distance);

        return point;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(currentDestination, 1f);
        Gizmos.DrawWireSphere(gameObject.transform.position, sightDistance);
        Gizmos.DrawWireSphere(gameObject.transform.position, meleeDistance);
    }

    public float lerpSpeed = 0.01f;

    public virtual void Update()
    {
        if (destinationTest != null)
        {
            destinationTest.transform.position = currentDestination;
        }
        return;

        if (active)
        {
            BehaviourTree();
        }

        if (agent.destination == currentDestination)
        {
            //Debug.Log("Moving");
            #region Animation

            Vector3 movement = new Vector3(agent.speed, 0, agent.speed) * Time.deltaTime;
            movement = transform.TransformDirection(movement);

            //Gets the rotation of the model to offset the animations
            Vector2 realMovement = new Vector2(0, 0);
            realMovement.x = Vector3.Dot(movement, model.right);
            realMovement.y = Vector3.Dot(movement, model.forward);

            //Sets the movement animations for the animator
            //Debug.Log("X:" + rb.velocity.x + "Y:" + rb.velocity.z);
            animator.SetFloat("xMovement", Mathf.Lerp(animator.GetFloat("xMovement"), realMovement.x, lerpSpeed));
            animator.SetFloat("yMovement", Mathf.Lerp(animator.GetFloat("yMovement"), realMovement.y, lerpSpeed));

            #endregion
        }
        else
        {
            //Debug.Log("Not Moving");
            animator.SetFloat("xMovement", 0);
            animator.SetFloat("yMovement", 0);
        }
    }

    public float sightDistance = 25;
    public float meleeDistance = 3;

    public virtual void BehaviourTree()
    {
        currentTarget = GetClosestEnemy(this);

        if (currentTarget != null)
        {
            currentDestination = currentTarget.transform.position;
            agent.SetDestination(currentDestination);
            AttackTarget(currentTarget);
        }
    }

    #region Behaviours

    protected bool AttackTarget(CharacterController targetCheck)
    {
        if (targetCheck == null)
            return false;

        float distance = Vector3.Distance(this.gameObject.transform.position, targetCheck.gameObject.transform.position);

        if (distance < meleeDistance)
        {
            combat.LightAttack();

            return true;
        }

        return false;
    }

    protected CharacterController GetClosestEnemy(CharacterController characterCheck)
    {
        CharacterController closestCharacter = null;
        float closestDistance = 99999;

        foreach (var item in AIManager.instance.GetEnemyTeam(this))
        {
            float itemDistance = Vector3.Distance(characterCheck.gameObject.transform.position, item.gameObject.transform.position);

            //Debug.Log(item.gameObject.name + " is " + itemDistance);

            if (itemDistance < sightDistance && itemDistance < closestDistance)
            {
                closestCharacter = item;
                closestDistance = itemDistance;
            }
        }

        return closestCharacter;
    }

    protected CharacterController GetFurthestEnemy(CharacterController characterCheck)
    {
        CharacterController closestCharacter = null;
        float closestDistance = 0;

        foreach (var item in AIManager.instance.GetEnemyTeam(this))
        {
            float itemDistance = Vector3.Distance(characterCheck.gameObject.transform.position, item.gameObject.transform.position);

            Debug.Log(item.gameObject.name + " is " + itemDistance);

            if (itemDistance < sightDistance && itemDistance > closestDistance)
            {
                closestCharacter = item;
                closestDistance = itemDistance;
            }
        }

        return closestCharacter;
    }

    #endregion
}
