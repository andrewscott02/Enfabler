using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;

    float xInput;
    float yInput;

    float xRotateInput;
    float yRotateInput;
    public float rotateInterval;
    public GameObject followTarget;

    PlayerMovement playerMovement;
    PlayerCombat playerCombat;

    //public LayerMask layerMask;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerMovement.animator = animator;
        playerCombat = GetComponent<PlayerCombat>();
        playerCombat.animator = animator;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCombat.canAttack)
        {
            xInput = Input.GetAxisRaw("Horizontal");
            yInput = Input.GetAxisRaw("Vertical");

            if (Input.GetButtonDown("Walk"))
            {
                playerMovement.ToggleSprint();
            }
        }

        if (Input.GetButtonDown("Light Attack"))
        {
            playerCombat.LightAttack();
        }

        if (Input.GetButtonDown("Heavy Attack"))
        {
            playerCombat.HeavyAttack();
        }

        //Camera Rotation
        xRotateInput = Input.GetAxisRaw("Mouse X");
        yRotateInput = Input.GetAxisRaw("Mouse Y");

        //Debug.Log(xRotateInput + " " + yRotateInput);

        transform.rotation *= Quaternion.AngleAxis(xRotateInput * rotateInterval, Vector3.up);

        followTarget.transform.rotation *= Quaternion.AngleAxis(yRotateInput * rotateInterval, Vector3.right);

        var angles = followTarget.transform.localEulerAngles;
        angles.z = 0;

        var angle = followTarget.transform.localEulerAngles.x;

        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if (angle < 180 && angle > 40)
        {
            angles.x = 40;
        }

        followTarget.transform.localEulerAngles = angles;

        if (xInput > 0 && yInput > 0)
        {
            transform.rotation = Quaternion.Euler(0, followTarget.transform.rotation.eulerAngles.y, 0);

            followTarget.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
        }
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
}
