using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;
    Rigidbody rb;
    public GameObject skeleton;

    [HideInInspector]
    public PlayerController controller;

    Transform model; public void SetModel(Transform newModel) { model = newModel; }

    bool sprinting = false;
    public float moveSpeed = 4;
    public float sprintSpeed = 8;
    float currentSpeed = 0;
    public float lerpSpeed = 0.01f;

    Vector3 movement = Vector3.zero;

    CinemachineVirtualCamera vCam;
    Cinemachine3rdPersonFollow vCamFollow;
    float defaultFOV;
    public float moveFOVMultiplier = 2;

    private void Start()
    {
        vCam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        vCamFollow = vCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        defaultFOV = vCam.m_Lens.FieldOfView;
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Moves the player and adjusts the animation
    /// </summary>
    /// <param name="xSpeed"> Determines the horizontal movement (Left and Right) </param>
    /// <param name="ySpeed"> Determines the vertical movement (Forward and Backward) </param>
    public void Move(Vector2 moveInput)
    {
        float xRemap = HelperFunctions.Remap(moveInput.x, -1, 1, 0, 1);

        if (moveInput != Vector2.zero)
        {
            //Rotate towards direction
            Vector3 moveInput3D = new Vector3(moveInput.x, 0, moveInput.y);
            Quaternion newRot = Quaternion.LookRotation(moveInput3D, Vector3.up) * Quaternion.Euler(0, controller.followTarget.transform.rotation.eulerAngles.y, 0);
            transform.rotation = newRot;
        }

        //Animate movement
        currentSpeed = moveInput.magnitude * (sprinting ? sprintSpeed : moveSpeed);
        currentSpeed = Mathf.Lerp(animator.GetFloat("RunBlend"), currentSpeed, lerpSpeed * Time.fixedDeltaTime);
        animator.SetFloat("RunBlend", currentSpeed);

        float newFOV = currentSpeed * moveFOVMultiplier;
        SetCameraValues(xRemap, defaultFOV + newFOV);
    }

    public void Sprint(bool sprinting)
    {
        Debug.Log("Sprinting: " + sprinting);
        this.sprinting = sprinting;
    }

    void SetCameraValues(float camSide, float desiredFOV)
    {
        vCamFollow.CameraSide = Mathf.Lerp(vCamFollow.CameraSide, camSide, Time.deltaTime);
        vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, desiredFOV, Time.deltaTime);
    }

    public FootStepData stepData;

    public void SpawnFootstep(int footTransformIndex)
    {
        Instantiate(stepData.footstepObject, stepData.footstepTransforms[footTransformIndex].position, new Quaternion(0, 0, 0, 0));

        SpawnImpulse((sprinting ? stepData.impulseSprintMultiplier : stepData.impulseWalkMultiplier) * currentSpeed);
    }

    void SpawnImpulse(float impulseStrength)
    {
        stepData.impulseSource.GenerateImpulseWithForce(impulseStrength);
    }

    public void DodgeRollLand(float impulseStrength)
    {
        SpawnImpulse(impulseStrength);
        RumbleManager.instance.ControllerRumble(0.25f, 1f, 0.25f);
    }
}