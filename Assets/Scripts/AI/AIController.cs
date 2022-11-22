using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : CharacterController
{
    protected GameObject player;
    protected NavMeshAgent agent;
    protected Vector3 currentDestination;

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
        NextPatrol();
        InvokeRepeating("NextPatrol", 0, 6f);
    }

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
    }

    public float lerpSpeed = 0.01f;

    public virtual void Update()
    {
        if (agent.destination == currentDestination)
        {
            Debug.Log("Moving");
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
            Debug.Log("Not Moving");
            animator.SetFloat("xMovement", 0);
            animator.SetFloat("yMovement", 0);
        }
    }
}
