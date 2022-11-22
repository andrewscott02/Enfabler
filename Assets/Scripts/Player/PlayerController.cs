using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Setup

    #region Variables

    public Animator animator;

    float xInput;
    float yInput;

    float xRotateInput;
    float yRotateInput;
    public Vector2 rotateInterval;
    public float rotateDeadZone = 0.1f;
    public GameObject followTarget;

    PlayerMovement playerMovement;
    PlayerCombat playerCombat;
    Health health;

    //public LayerMask layerMask;

    #endregion

    private void Start()
    {
        health = GetComponent<Health>();
        playerMovement = GetComponent<PlayerMovement>();
        playerMovement.animator = animator;
        playerCombat = GetComponent<PlayerCombat>();
        playerCombat.animator = animator;

        Health allyHealth = GameObject.FindObjectOfType<ConstructPlayerModel>().GetComponent<Health>();

        playerCombat.ignore.Add(health);
        playerCombat.ignore.Add(allyHealth);
    }

    #endregion

    #region Inputs

    // Update is called once per frame
    void Update()
    {
        #region Movement

        if (playerCombat.canMove)
        {
            xInput = Input.GetAxisRaw("Horizontal");
            yInput = Input.GetAxisRaw("Vertical");

            if (Input.GetButtonDown("Walk"))
            {
                playerMovement.ToggleSprint();
            }
        }

        #endregion

        #region Actions

        if (Input.GetButtonDown("Light Attack"))
        {
            playerCombat.LightAttack();
        }

        if (Input.GetButtonDown("Parry"))
        {
            playerCombat.Parry();
        }

        if (Input.GetButtonDown("Dodge"))
        {
            playerCombat.Dodge();
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
        if (playerCombat.canMove)
        {
            /*
            if (((xInput >= 0.1) || (xInput <= -0.1)) && ((yInput >= 0.1) || (yInput <= -0.1)))
            {
                //Partial functionality for averaging diagonal movement, needs proper implementation
                //Debug.Log(true);
                xInput = xInput / 2;
                yInput = yInput / 2;
            }
            */

            playerMovement.Move(xInput * 2, yInput * 2);
        }

        playerMovement.animator.SetBool("CanMove", playerCombat.canMove);
    }

    #endregion
}
