using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;
    Rigidbody rb;
    Transform model; public void SetModel(Transform newModel) { model = newModel; }

    bool isSprinting = false;
    public float walkSpeed = 160;
    public float runSpeed = 300;
    public float dodgeSpeed = 300;

    public float lerpSpeed = 0.01f;

    public Vector3 movement;

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
        animator.SetFloat("RunBlendX", Mathf.Lerp(animator.GetFloat("RunBlendX"), xSpeed, lerpSpeed));
        animator.SetFloat("RunBlendY", Mathf.Lerp(animator.GetFloat("RunBlendY"), ySpeed, lerpSpeed));
    }

    public void ToggleSprint()
    {
        isSprinting = !isSprinting;
    }
}