using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterController
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
        playerMovement.animator = animator;
        playerMovement.SetModel(model);

        //AIManager.instance.AllocateTeam(this);
    }

    #endregion

    #region Inputs

    public void MoveInput(InputAction.CallbackContext context)
    {
        if (combat.canMove)
        {
            moveInput.x = context.ReadValue<Vector2>().x;
            moveInput.y = context.ReadValue<Vector2>().y;
        }
        else
        {
            moveInput.x = 0;
            moveInput.y = 0;
        }
    }

    public void CameraInput(InputAction.CallbackContext context)
    {
        xRotateInput = context.ReadValue<Vector2>().x;
        yRotateInput = context.ReadValue<Vector2>().y;
    }

    public void AttackInput(InputAction.CallbackContext context)
    {
        combat.LightAttack();
    }

    public void BlockInput(InputAction.CallbackContext context)
    {
        combat.Block();
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

        transform.rotation = Quaternion.Euler(0, followTarget.transform.rotation.eulerAngles.y, 0);

        followTarget.transform.localEulerAngles = new Vector3(angles.x, 0, 0);

        #endregion
    }

    private void FixedUpdate()
    {
        //Debug.Log(xInput + "|| " + yInput);
        playerMovement.Move(moveInput);
    }

    #endregion
}
