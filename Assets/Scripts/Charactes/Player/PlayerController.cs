using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : BaseCharacterController
{
    #region Setup

    #region Variables

    Vector2 moveInput;

    float xRotateInput;
    float yRotateInput;
    public Vector2 rotateInterval;
    public float rotateDeadZone = 0.1f;
    public GameObject followTarget;

    PlayerMovement playerMovement;

    //public LayerMask layerMask;

    #endregion

    public override void Start()
    {
        base.Start();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerMovement = GetComponent<PlayerMovement>();
        playerMovement.controller = this;
        playerMovement.animator = animator;
        playerMovement.SetModel(model);

        //AIManager.instance.AllocateTeam(this);
    }

    #endregion

    #region Inputs

    public void MoveInput(InputAction.CallbackContext context)
    {
        moveInput.x = context.ReadValue<Vector2>().x;
        moveInput.y = context.ReadValue<Vector2>().y;
    }

    public void CameraInput(InputAction.CallbackContext context)
    {
        xRotateInput = context.ReadValue<Vector2>().x;
        yRotateInput = context.ReadValue<Vector2>().y;
    }

    public void AttackInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (moveInput != Vector2.zero && combat.canAttack)
        {
            //Rotate towards direction
            Vector3 moveInput3D = new Vector3(moveInput.x, 0, moveInput.y);
            Quaternion newRot = Quaternion.LookRotation(moveInput3D, Vector3.up) * Quaternion.Euler(0, followTarget.transform.rotation.eulerAngles.y, 0);
            transform.rotation = newRot;
        }

        combat.LightAttack();
    }

    public void BlockInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (moveInput != Vector2.zero)
        {
            //Rotate towards direction
            Vector3 moveInput3D = new Vector3(moveInput.x, 0, moveInput.y);
            Quaternion newRot = Quaternion.LookRotation(moveInput3D, Vector3.up) * Quaternion.Euler(0, followTarget.transform.rotation.eulerAngles.y, 0);
            transform.rotation = newRot;
        }

        combat.Block();
    }

    public void DodgeInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (moveInput != Vector2.zero && combat.canDodge)
        {
            //Rotate towards direction
            Vector3 moveInput3D = new Vector3(moveInput.x, 0, moveInput.y);
            Quaternion newRot = Quaternion.LookRotation(moveInput3D, Vector3.up) * Quaternion.Euler(0, followTarget.transform.rotation.eulerAngles.y, 0);
            transform.rotation = newRot;
        }

        combat.Dodge();
    }

    public void SprintInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            playerMovement.Sprint(true);

        if (context.canceled)
            playerMovement.Sprint(false);
    }

    // Update is called once per frame
    void Update()
    {
        #region Camera Rotation

        followTarget.transform.rotation *= Quaternion.AngleAxis(xRotateInput * rotateInterval.x, Vector3.up);

        followTarget.transform.rotation *= Quaternion.AngleAxis(yRotateInput * rotateInterval.y, Vector3.right);

        Vector3 angles = followTarget.transform.localEulerAngles;
        angles.z = 0;

        float angle = followTarget.transform.localEulerAngles.x;

        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if (angle < 180 && angle > 40)
        {
            angles.x = 40;
        }

        followTarget.transform.localEulerAngles = angles;

        #endregion
    }

    private void FixedUpdate()
    {
        if (combat.canMove)
        {
            playerMovement.Move(moveInput);
        }
        else
        {
            playerMovement.Move(Vector2.zero);
        }
    }

    #endregion
}
