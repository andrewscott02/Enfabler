using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;
    Rigidbody rb;
    public Transform model;

    bool isSprinting = false;
    public float walkSpeed;
    public float runSpeed;

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
    public void Move(float xSpeed, float ySpeed)
    {
        //Debug.Log("Move: " + xSpeed + " | " + ySpeed);
        //Movement
        Vector3 movement = new Vector3(xSpeed, 0, ySpeed) * Time.deltaTime;
        movement = transform.TransformDirection(movement);

        if (!isSprinting)
        {
            movement *= walkSpeed;
        }
        else
        {
            movement *= runSpeed;
        }

        rb.velocity = movement;

        #region Animation

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

    public void ToggleSprint()
    {
        isSprinting = !isSprinting;
    }
}