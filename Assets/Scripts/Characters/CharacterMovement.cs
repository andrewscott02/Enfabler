using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public bool sprinting { get; protected set; } = false;
    public float currentSpeed = 0;

    public float gravityMultiplier = 5f;

    public FootStepData stepData;

    protected Rigidbody rb;

    public LayerMask groundLayer;
    protected bool grounded;

    public float rotateSpeed = 1000;
    public Quaternion targetRotation;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void FixedUpdate()
    {
        grounded = Physics.Raycast(transform.position + new Vector3(0, 0.2f, 0), Vector3.down, 0.4f, groundLayer);

        if (!grounded)
            rb.AddForce(Physics.gravity * gravityMultiplier * Time.fixedDeltaTime, ForceMode.Impulse);
    }

    public void SpawnFootstep(int footTransformIndex)
    {
        if (Vector3.Distance(Camera.main.transform.position, this.transform.position) <= stepData.footstepRange) 
            Instantiate(stepData.footstepObject, stepData.footstepTransforms[footTransformIndex].position, new Quaternion(0, 0, 0, 0));

        SpawnImpulse((sprinting ? stepData.impulseSprintMultiplier : stepData.impulseWalkMultiplier) * currentSpeed);
    }

    void SpawnImpulse(float impulseStrength)
    {
        if (Vector3.Distance(Camera.main.transform.position, this.transform.position) > stepData.impulseRange) return;
        //Debug.Log(gameObject.name + " Spawn impulse with strength + " + impulseStrength);
        stepData.impulseSource.GenerateImpulseWithForce(impulseStrength);
    }

    public void DodgeRollLand(float impulseStrength)
    {
        if (Vector3.Distance(Camera.main.transform.position, this.transform.position) > stepData.impulseRange) return;

        SpawnImpulse(impulseStrength);
        RumbleManager.instance.ControllerRumble(0.25f, 1f, 0.25f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + new Vector3(0, 0.2f, 0), transform.position + new Vector3(0, 0.2f, 0) + (Vector3.down * 0.4f));

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stepData.impulseRange);
    }
}
