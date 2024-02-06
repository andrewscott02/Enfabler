using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : PuzzleElement
{
    public E_MovementType rotateType;
    public float rotateDelay = 1f;
    public float rotateSpeed = 5f;
    public Vector3 angleOffset;

    Vector3 startRotation, endRotation;
    Vector3 targetRotation;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        startRotation = transform.rotation.eulerAngles;
        endRotation = startRotation + angleOffset;

        switch (rotateType)
        {
            case E_MovementType.Toggle:
                rotating = true;
                StartCoroutine(IRotateToRotation(startRotation, rotateDelay));
                break;
            case E_MovementType.EnableMovement:
                rotating = false;
                targetRotation = endRotation;
                break;
            case E_MovementType.EnableMovementReversable:
                rotating = false;
                targetRotation = startRotation;
                break;
            case E_MovementType.EnableMovementEndless:
                rotating = false;
                targetRotation = angleOffset;
                break;
        }
    }

    protected override void Activate()
    {
        Debug.Log("Unlock Interaction");

        StopAllCoroutines();

        switch (rotateType)
        {
            case E_MovementType.Toggle:
                StartCoroutine(IRotateToRotation(endRotation, rotateDelay));
                break;
            case E_MovementType.EnableMovement:
                EnableRotation(true);
                break;
            case E_MovementType.EnableMovementReversable:
                EnableRotation(true);
                break;
            case E_MovementType.EnableMovementEndless:
                EnableRotation(true);
                break;
        }
    }

    protected override void Deactivate()
    {
        Debug.Log("Unlock Interaction");

        StopAllCoroutines();

        switch (rotateType)
        {
            case E_MovementType.Toggle:
                StartCoroutine(IRotateToRotation(startRotation, rotateDelay));
                break;
            case E_MovementType.EnableMovement:
                EnableRotation(false);
                break;
            case E_MovementType.EnableMovementReversable:
                EnableRotation(false);
                break;
            case E_MovementType.EnableMovementEndless:
                EnableRotation(false);
                break;
        }
    }

    IEnumerator IRotateToRotation(Vector3 rotation, float delay)
    {
        targetRotation = transform.rotation.eulerAngles;

        yield return new WaitForSeconds(delay);

        targetRotation = rotation;
    }

    bool rotating = false;

    void EnableRotation(bool enabled)
    {
        rotating = enabled;
    }

    private void FixedUpdate()
    {
        if (!rotating) return;

        if (rotateType == E_MovementType.EnableMovementEndless)
        {
            RotateInDirection();
            return;
        }

        return;
        //TODO: Following not implemented

        if (!HelperFunctions.AlmostEqualVector3(targetRotation, transform.position, 0.05f, Vector3.zero))
        {
            Vector3 dir = (targetRotation - transform.position).normalized;

            transform.position += dir * Time.fixedDeltaTime * rotateSpeed;
        }
        else
        {
            if (rotateType == E_MovementType.EnableMovementReversable)
            {
                if (targetRotation == startRotation)
                {
                    targetRotation = endRotation;
                }
                else if (targetRotation == endRotation)
                {
                    targetRotation = startRotation;
                }
            }
        }
    }

    void RotateInDirection()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + (targetRotation * Time.fixedDeltaTime * rotateSpeed));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.2f);
        Gizmos.DrawWireSphere(transform.position + angleOffset, 0.2f);
    }
}