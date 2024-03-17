using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Enfabler.Attacking;

public class PlayerController : BaseCharacterController
{
    #region Setup

    #region Variables

    Vector2 moveInput;

    float xRotateInput;
    float yRotateInput;
    public Vector2 yRotateThresholds = new Vector2(340, 40);
    public Vector2 rotateIntervalGamePad, rotateIntervalMouse;
    public float rotateDeadZone = 0.1f;
    public FollowTarget followTarget;

    bool useMoveRotation = false;
    float xMoveRotateInput;
    public float moveRotateMultiplier = 0.1f;

    PlayerMovement playerMovement;

    //public LayerMask layerMask;

    #endregion

    public override void Awake()
    {
        base.Awake();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerMovement = GetComponent<PlayerMovement>();
        playerMovement.controller = this;
        playerMovement.animator = animator;
        playerMovement.SetModel(model);
        playerInput = GetComponent<PlayerInput>();
        CheckInput();

        AIManager.instance.AllocateTeam(this);
    }

    #endregion

    #region Inputs

    PlayerInput playerInput;

    #region Inputs - Movement and Camera

    public void MoveInput(InputAction.CallbackContext context)
    {
        if (PauseMenu.instance.paused) return;

        moveInput.x = context.ReadValue<Vector2>().x;
        moveInput.y = context.ReadValue<Vector2>().y;

        xMoveRotateInput = context.ReadValue<Vector2>().x * moveRotateMultiplier;
    }

    public void CameraInput(InputAction.CallbackContext context)
    {
        if (PauseMenu.instance.paused)
        {
            xRotateInput = 0;
            yRotateInput = 0;

            return;
        }

        xRotateInput = context.ReadValue<Vector2>().x;
        yRotateInput = context.ReadValue<Vector2>().y;
    }

    Coroutine sprintCoroutine;

    public void SprintInput(InputAction.CallbackContext context)
    {
        if (!gameObject.activeSelf) return;

        if (PauseMenu.instance.paused)
        {
            if (sprintCoroutine != null)
                StopCoroutine(sprintCoroutine);
            return;
        }

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

    #endregion

    #region Inputs - Attacks

    public void PrimaryAttackInput(InputAction.CallbackContext context)
    {
        Attack(context, E_AttackType.PrimaryAttack);
    }

    public void SecondaryAttackInput(InputAction.CallbackContext context)
    {
        Attack(context, E_AttackType.SecondaryAttack);
    }

    public void Attack(InputAction.CallbackContext context, E_AttackType attackType)
    {
        if (PauseMenu.instance.paused) return;

        if (context.performed)
        {
            if (moveInput != Vector2.zero && (combat.canAttack || (combat.canSaveAttackInput && combat.savingAttackInput == E_AttackType.None)))
            {
                //Rotate towards direction
                Vector3 moveInput3D = new Vector3(moveInput.x, 0, moveInput.y);
                Quaternion newRot = Quaternion.LookRotation(moveInput3D, Vector3.up) * Quaternion.Euler(0, followTarget.transform.rotation.eulerAngles.y, 0);
                transform.rotation = newRot;
            }
            combat.savingChargeInput = attackType;
            combat.Attack(attackType: attackType);
        }
        else if (context.canceled)
        {
            combat.ReleaseAttack();
        }
    }

    #endregion

    #region Inputs - Defence

    public void BlockInput(InputAction.CallbackContext context)
    {
        if (PauseMenu.instance.paused) return;

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
        if (PauseMenu.instance.paused) return;

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

    #endregion

    #region Inputs - Interaction

    public void InteractInput(InputAction.CallbackContext context)
    {
        if (PauseMenu.instance.paused) return;

        if (!context.performed || !combat.canAttack)
            return;

        if (!enableInteraction) return;

        //Debug.Log("Interact");
        combat.ForceEndAttack();
        animator.SetTrigger(interactAnim);

        interactable.Interacted(this);
    }

    IInteractable interactable;
    bool enableInteraction = false;
    string interactAnim;

    public void EnableInteraction(E_InteractTypes interactType, IInteractable interactable = null)
    {
        enableInteraction = interactable != null;
        this.interactable = interactable;
        interactAnim = interactType.ToString();
    }

    #endregion

    #region Inputs - Pause Menu

    public void PauseInput(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (PauseMenu.instance.paused) return;

        PauseMenu.instance.PauseGame();
    }

    public void Close(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!PauseMenu.instance.paused) return;

        PauseMenu.instance.Resume();
    }

    public void NextPage(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!PauseMenu.instance.paused) return;

        PauseMenu.instance.ChangePage(true);
    }

    public void PreviousPage(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!PauseMenu.instance.paused) return;

        PauseMenu.instance.ChangePage(false);
    }

    #endregion

    #region Inputs - Other

    public static bool usingGamepad = false;

    void CheckInput()
    {
        usingGamepad = playerInput.currentControlScheme == "Gamepad";
    }

    public void OnControlsChange(PlayerInput input)
    {
        usingGamepad = input.currentControlScheme == "Gamepad";

        if (PauseMenu.instance != null)
            PauseMenu.instance.onControlsChange(input);
    }

    #endregion

    private void Update()
    {
        #region Camera Rotation

        float xRotateValue = xRotateInput * Time.deltaTime;
        float yRotateValue = yRotateInput * Time.deltaTime;

        if (useMoveRotation)
        {
            if (xRotateInput < 0.5f && xRotateInput > -0.5f)
                xRotateValue = xMoveRotateInput;
        }

        Vector2 rotateInterval = usingGamepad ? rotateIntervalGamePad : rotateIntervalMouse;

        followTarget.transform.rotation *= Quaternion.AngleAxis(xRotateValue * rotateInterval.x, Vector3.up);

        followTarget.transform.rotation *= Quaternion.AngleAxis(yRotateValue * rotateInterval.y, Vector3.right);

        Vector3 angles = followTarget.transform.localEulerAngles;
        angles.z = 0;

        float angle = followTarget.transform.localEulerAngles.x;

        if (angle > 180 && angle < yRotateThresholds.x)
        {
            angles.x = yRotateThresholds.x;
        }
        else if (angle < 180 && angle > yRotateThresholds.y)
        {
            angles.x = yRotateThresholds.y;
        }

        followTarget.transform.localEulerAngles = angles;

        #endregion

        if (combat.canMove || combat.chargingAttack != E_AttackType.None)
        {
            playerMovement.Move(moveInput);
        }
        else
        {
            playerMovement.Move(Vector2.zero);
        }
    }

    #endregion

    public override void ActivateRagdoll(bool activate, ExplosiveForceData forceData, bool disableAnimator)
    {
        followTarget.follow.parent = activate ? null : followTarget.follow.transform;

        base.ActivateRagdoll(activate, forceData, disableAnimator);
    }
}
