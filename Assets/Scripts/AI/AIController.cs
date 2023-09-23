using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTrees;

public class AIController : BaseCharacterController
{
    #region Setup

    protected GameObject player; public GameObject GetPlayer() { return player; }

    #region Behaviour Tree
    protected NavMeshAgent agent; public NavMeshAgent GetNavMeshAgent() { return agent; }
    public BehaviourTree bt;


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

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(currentDestination, distanceAllowance);

        Gizmos.DrawWireSphere(gameObject.transform.position, chaseDistance);
        Gizmos.DrawWireSphere(gameObject.transform.position, roamDistance);
        Gizmos.DrawWireSphere(gameObject.transform.position, maxDistanceFromModelCharacter);
        Gizmos.DrawWireSphere(gameObject.transform.position, meleeDistance);

        if (currentTarget != null)
        {
            Gizmos.DrawLine(transform.position, currentTarget.transform.position);
        }
    }

    public virtual void Update()
    {
        if (currentTarget != null)
        {
            Vector3 direction = (currentTarget.transform.position - transform.position).normalized;

            Quaternion desiredrot = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, desiredrot, Time.deltaTime * agent.angularSpeed);
        }

        #region Animation

        Vector3 movement = agent.velocity;
        //movement = transform.TransformDirection(movement);

        //Gets the rotation of the model to offset the animations
        Vector2 realMovement = new Vector2(0, 0);
        realMovement.x = Vector3.Dot(movement, model.right);
        realMovement.y = Vector3.Dot(movement, model.forward);

        //Sets the movement animations for the animator
        //Debug.Log("X:" + rb.velocity.x + "Y:" + rb.velocity.z);
        animator.SetFloat("xMovement", Mathf.Lerp(animator.GetFloat("xMovement"), realMovement.x, lerpSpeed));
        animator.SetFloat("yMovement", Mathf.Lerp(animator.GetFloat("yMovement"), realMovement.y, lerpSpeed));

        #endregion

        if (currentCooldown > timeSinceLastAttack)
        {
            timeSinceLastAttack += Time.deltaTime;
        }

        lastParry += Time.deltaTime;
        lastDodge += Time.deltaTime;
    }

    public float distanceAllowance = 1f;

    public float lerpSpeed = 0.01f;

    public float sightDistance = 100;
    public float chaseDistance = 40;
    public float roamDistance = 25;
    public float maxDistanceFromModelCharacter = 6;
    public float meleeDistance = 3;

    public Vector3 followVector;
    public float followDistance = 5;

    #endregion

    #region Behaviours

    #region Movement

    public float walkSpeed = 6;
    public float sprintSpeed = 10;

    protected Vector3 currentDestination; public Vector3 GetDestination() { return currentDestination; }
    public void SetDestinationPos(Vector3 pos)
    {
        currentDestination = pos;
    }
    public bool roaming = false;
    public void MoveToDestination(bool sprinting)
    {
        agent.speed = sprinting ? sprintSpeed : walkSpeed;

        agent.SetDestination(currentDestination);
    }
    public bool NearDestination(float distanceAllowance)
    {
        return Vector3.Distance(transform.position, currentDestination) <= distanceAllowance;
    }

    bool NearDestination()
    {
        return Vector3.Distance(transform.position, currentDestination) < distanceAllowance;
    }

    #endregion

    #region Combat

    public BaseCharacterController currentTarget;
    BaseCharacterController lastAttacked;

    bool doubleAttack;
    public float doubleAttackChance;
    public Vector2 attackCooldown;
    float currentCooldown;
    float timeSinceLastAttack;

    public bool CanAttack()
    {
        if (currentTarget == null)
            return false;

        if (combat.canAttack && timeSinceLastAttack >= currentCooldown)
        {
            return true;
        }

        return false;
    }

    public bool AttackTarget()
    {
        if (currentTarget == null)
            return false;

        float distance = Vector3.Distance(this.gameObject.transform.position, currentTarget.gameObject.transform.position);
        //Debug.Log("Attack called");
        if (distance < meleeDistance)
        {
            if (combat.canAttack && timeSinceLastAttack >= currentCooldown)
            {
                if (doubleAttack)
                    doubleAttack = false;
                else
                    doubleAttack = Random.Range(0f, 1f) < doubleAttackChance;

                lastAttacked = currentTarget;
                //lastAttacked.GetCharacterCombat().StartBeingAttacked();

                //Debug.Log("Attack made");
                combat.LightAttack();
                timeSinceLastAttack = 0;

                if (doubleAttack)
                {
                    currentCooldown = 0;
                }
                else
                {
                    currentCooldown = Random.Range(attackCooldown.x, attackCooldown.y);
                }
            }

            return true;
        }

        return false;
    }

    public void EndAttackOnTarget()
    {
        if (lastAttacked != null)
        {
            //lastAttacked.GetCharacterCombat().StopBeingAttacked();
            lastAttacked = null;
        }
    }

    public float parryChance = 0f;
    public float parryCooldown = 1f;
    float lastParry;

    public bool CanParry()
    {
        if (lastParry > parryCooldown)
        {
            lastParry = 0;
            return (Random.Range(0f, 1f) < parryChance) && combat.canParry;
        }

        return false;
    }

    public float dodgeChance = 0f;
    public float dodgeCooldown = 1f;
    float lastDodge;

    public bool CanDodge()
    {
        if (lastDodge > dodgeCooldown)
        {
            lastDodge = 0;
            return (Random.Range(0f, 1f) < parryChance) && combat.canParry;
        }

        return false;
    }

    #endregion

    #endregion
}
