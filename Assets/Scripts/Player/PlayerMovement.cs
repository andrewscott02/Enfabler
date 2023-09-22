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

    bool isSprinting = false;
    public float walkSpeed = 4;
    public float runSpeed = 8;
    public float dodgeSpeed = 8;

    public float lerpSpeed = 0.01f;

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
        moveInput.Normalize();
        Vector3 movement = new Vector3(moveInput.x * runSpeed, 0, moveInput.y * runSpeed) * Time.deltaTime;

        if (moveInput != Vector2.zero)
        {
            //Rotate towards direction
            Vector3 moveInput3D = new Vector3(moveInput.x, 0, moveInput.y);
            Quaternion newRot = Quaternion.LookRotation(moveInput3D, Vector3.up) * Quaternion.Euler(0, controller.followTarget.transform.rotation.eulerAngles.y, 0);
            skeleton.transform.rotation = newRot;

            movement = Quaternion.AngleAxis(controller.followTarget.transform.rotation.eulerAngles.y, Vector3.up) * movement;
            rb.velocity = transform.TransformDirection(movement);
        }

        //Animate movement
        float magnitude = moveInput.magnitude * runSpeed;
        animator.SetFloat("RunBlend", Mathf.Lerp(animator.GetFloat("RunBlend"), magnitude, lerpSpeed));
    }

    public void ToggleSprint()
    {
        isSprinting = !isSprinting;
    }
}