using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;
    Rigidbody rb;
    public GameObject skeleton;

    [HideInInspector]
    public PlayerController controller;

    Transform model; public void SetModel(Transform newModel) { model = newModel; }

    public float animSpeed = 7;
    public float moveSpeed = 600;

    public float lerpSpeed = 0.01f;

    Vector3 movement = Vector3.zero;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Moves the player and adjusts the animation
    /// </summary>
    /// <param name="xSpeed"> Determines the horizontal movement (Left and Right) </param>
    /// <param name="ySpeed"> Determines the vertical movement (Forward and Backward) </param>
    public void Move(Vector2 moveInput)
    {
        if (!controller.GetCharacterCombat().canMove)
            moveInput = Vector2.zero;

        if (moveInput != Vector2.zero)
        {
            //Rotate towards direction
            Vector3 moveInput3D = new Vector3(moveInput.x, 0, moveInput.y);
            Quaternion newRot = Quaternion.LookRotation(moveInput3D, Vector3.up) * Quaternion.Euler(0, controller.followTarget.transform.rotation.eulerAngles.y, 0);
            skeleton.transform.rotation = newRot;

            //Determine velocity
            movement = new Vector3(moveInput.x * moveSpeed, 0, moveInput.y * moveSpeed) * Time.fixedDeltaTime;
            movement = Quaternion.AngleAxis(controller.followTarget.transform.rotation.eulerAngles.y, Vector3.up) * movement;
            Vector3 lerpVector = HelperFunctions.LerpVector3(rb.velocity, transform.TransformDirection(movement), lerpSpeed);
            rb.velocity = lerpVector;
        }

        //Animate movement
        float magnitude = moveInput.magnitude * animSpeed;
        animator.SetFloat("RunBlend", Mathf.Lerp(animator.GetFloat("RunBlend"), magnitude, lerpSpeed));
    }

    public FootStepData stepData;

    public void SpawnFootstep(int footTransformIndex)
    {
        Instantiate(stepData.footstepObject, stepData.footstepTransforms[footTransformIndex].position, new Quaternion(0, 0, 0, 0));
    }
}