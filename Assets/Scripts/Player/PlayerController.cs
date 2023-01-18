using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterController
{
    #region Setup

    #region Variables

    float xInput;
    float yInput;

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
        playerMovement = GetComponent<PlayerMovement>();
        playerMovement.animator = animator;
        playerMovement.SetModel(model);

        Health allyHealth = GameObject.FindObjectOfType<ConstructPlayerModel>().GetComponent<Health>();

        combat.ignore.Add(allyHealth);

        AIManager.instance.AllocateTeam(this);
    }

    #endregion

    #region Inputs

    // Update is called once per frame
    void Update()
    {
        #region Movement

        if (combat.canMove)
        {
            xInput = Input.GetAxisRaw("Horizontal");
            yInput = Input.GetAxisRaw("Vertical");

            if (Input.GetButtonDown("Walk"))
            {
                playerMovement.ToggleSprint();
            }
        }
        else
        {
            xInput = 0;
            yInput = 0;
        }

        #endregion

        #region Actions

        if (Input.GetButtonDown("Light Attack"))
        {
            combat.LightAttack();
        }

        if (Input.GetButtonDown("Parry"))
        {
            combat.Parry();
        }

        if (Input.GetButtonDown("Dodge"))
        {
            combat.Dodge();
        }

        #endregion

        #region Camera Rotation

        xRotateInput = Input.GetAxisRaw("Mouse X");
        yRotateInput = Input.GetAxisRaw("Mouse Y");

        //Debug.Log(xRotateInput + " " + yRotateInput);

        transform.rotation *= Quaternion.AngleAxis(xRotateInput * rotateInterval.x, Vector3.up);

        #region Y Rotation

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

        if (xRotateInput > rotateDeadZone || xRotateInput > -rotateDeadZone)
        {
            transform.rotation = Quaternion.Euler(0, followTarget.transform.rotation.eulerAngles.y, 0);

            followTarget.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
        }

        #endregion

        #endregion
    }

    private void FixedUpdate()
    {
        playerMovement.Move(xInput * 2, yInput * 2);

        playerMovement.animator.SetBool("CanMove", combat.canMove);
    }

    #endregion
}
