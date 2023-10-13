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

    bool useMoveRotation = false;
    float xMoveRotateInput;
    float moveRotateMultiplier = 0.2f;

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

        AIManager.instance.AllocateTeam(this);
    }

    #endregion

    #region Inputs

    public void MoveInput(InputAction.CallbackContext context)
    {
        moveInput.x = context.ReadValue<Vector2>().x;
        moveInput.y = context.ReadValue<Vector2>().y;

        xMoveRotateInput = context.ReadValue<Vector2>().x * moveRotateMultiplier;
    }

    public void CameraInput(InputAction.CallbackContext context)
    {
        xRotateInput = context.ReadValue<Vector2>().x;
        yRotateInput = context.ReadValue<Vector2>().y;
    }

    public void AttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (moveInput != Vector2.zero && (combat.canAttack || (combat.canSaveAttackInput && !combat.savingAttackInput)))
            {
                //Rotate towards direction
                Vector3 moveInput3D = new Vector3(moveInput.x, 0, moveInput.y);
                Quaternion newRot = Quaternion.LookRotation(moveInput3D, Vector3.up) * Quaternion.Euler(0, followTarget.transform.rotation.eulerAngles.y, 0);
                transform.rotation = newRot;
            }
            combat.savingChargeInput = true;
            combat.LightAttack();
        }
        else if (context.canceled)
        {
            combat.ReleaseAttack();
        }
    }

    public void BlockInput(InputAction.CallbackContext context)
    {
        if (moveInput != Vector2.zero)
        {
            //Rotate towards direction
            Vector3 moveInput3D = new Vector3(moveInput.x, 0, moveInput.y);
            Quaternion newRot = Quaternion.LookRotation(moveInput3D, Vector3.up) * Quaternion.Euler(0, followTarget.transform.rotation.eulerAngles.y, 0);
            transform.rotation = newRot;
        }

        if (context.performed)
        {
            combat.Block(true);
        }

        if (context.canceled)
        {
            combat.Block(false);
        }
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

    Coroutine sprintCoroutine;

    public void SprintInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerMovement.Sprint(true);
            combat.sprinting = true;
            sprintCoroutine = StartCoroutine(IDelayUseMoveCam(1.5f));
        }

        if (context.canceled)
        {
            playerMovement.Sprint(false);
            if (sprintCoroutine != null)
                StopCoroutine(sprintCoroutine);
            StartCoroutine(IDelayStopSprint(0.5f));
            useMoveRotation = false;
        }
    }

    IEnumerator IDelayUseMoveCam(float delay)
    {
        yield return new WaitForSeconds(delay);
        useMoveRotation = true;
    }

    IEnumerator IDelayStopSprint(float delay)
    {
        yield return new WaitForSeconds(delay);
        combat.sprinting = false;
    }

    public void InteractInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        Debug.Log("Interact");
        animator.SetTrigger("Interact");
        //TODO: Proper interact interface and checks
    }

    // Update is called once per frame
    void Update()
    {
        #region Camera Rotation

        float xRotateValue = xRotateInput;

        if (useMoveRotation)
        {
            if (xRotateInput < 0.5f && xRotateInput > -0.5f)
                xRotateValue = xMoveRotateInput;
        }

        followTarget.transform.rotation *= Quaternion.AngleAxis(xRotateValue * rotateInterval.x, Vector3.up);

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
        if (combat.canMove || combat.chargingAttack)
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
