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

    public float moveSpeed = 150;

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

        moveInput.Normalize();
        movement = HelperFunctions.LerpVector3(movement, new Vector3(moveInput.x * moveSpeed, 0, moveInput.y * moveSpeed) * Time.deltaTime, lerpSpeed);

        if (moveInput != Vector2.zero)
        {
            //Rotate towards direction
            Vector3 moveInput3D = new Vector3(moveInput.x, 0, moveInput.y);
            Quaternion newRot = Quaternion.LookRotation(moveInput3D, Vector3.up) * Quaternion.Euler(0, controller.followTarget.transform.rotation.eulerAngles.y, 0);
            skeleton.transform.rotation = newRot;

            movement = Quaternion.AngleAxis(controller.followTarget.transform.rotation.eulerAngles.y, Vector3.up) * movement;

            //TODO:Lerp velocity here
            rb.velocity = transform.TransformDirection(movement);
        }

        //Animate movement
        float magnitude = moveInput.magnitude * moveSpeed;
        animator.SetFloat("RunBlend", Mathf.Lerp(animator.GetFloat("RunBlend"), magnitude, lerpSpeed));
    }
}