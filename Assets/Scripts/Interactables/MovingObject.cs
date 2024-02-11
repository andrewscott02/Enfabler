using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public E_MovementType movementType;
    public float moveDelay = 1f;
    public float moveSpeed = 5f;
    public Vector3 endOffset;

    Vector3 startPosition, endPosition;
    Vector3 targetPosition;

    // Start is called before the first frame update
    protected void Start()
    {
        startPosition = transform.position;
        endPosition = startPosition + GetMovePosition(endOffset);

        switch (movementType)
        {
            case E_MovementType.Toggle:
                moving = true;
                StartCoroutine(IMoveToPosition(startPosition, moveDelay));
                break;
            case E_MovementType.EnableMovement:
                moving = false;
                targetPosition = endPosition;
                break;
            case E_MovementType.EnableMovementReversable:
                moving = false;
                targetPosition = startPosition;
                break;
            case E_MovementType.EnableMovementEndless:
                moving = false;
                targetPosition = GetMovePosition(endOffset);
                break;
        }

        Activate();
    }

    protected void Activate()
    {
        Debug.Log("Unlock Interaction");

        StopAllCoroutines();

        switch (movementType)
        {
            case E_MovementType.Toggle:
                StartCoroutine(IMoveToPosition(endPosition, moveDelay));
                break;
            case E_MovementType.EnableMovement:
                EnableMovement(true);
                break;
            case E_MovementType.EnableMovementReversable:
                EnableMovement(true);
                break;
            case E_MovementType.EnableMovementEndless:
                EnableMovement(true);
                break;
        }
    }

    protected void Deactivate()
    {
        Debug.Log("Unlock Interaction");

        StopAllCoroutines();

        switch (movementType)
        {
            case E_MovementType.Toggle:
                StartCoroutine(IMoveToPosition(startPosition, moveDelay));
                break;
            case E_MovementType.EnableMovement:
                EnableMovement(false);
                break;
            case E_MovementType.EnableMovementReversable:
                EnableMovement(false);
                break;
            case E_MovementType.EnableMovementEndless:
                EnableMovement(false);
                break;
        }
    }

    IEnumerator IMoveToPosition(Vector3 position, float delay)
    {
        targetPosition = transform.position;

        yield return new WaitForSeconds(delay);

        targetPosition = position;
    }

    bool moving = false;

    void EnableMovement(bool enabled)
    {
        moving = enabled;
    }

    private void FixedUpdate()
    {
        if (!moving) return;

        if (movementType == E_MovementType.EnableMovementEndless)
        {
            MoveInDirection();
            return;
        }

        if (!HelperFunctions.AlmostEqualVector3(targetPosition, transform.position, 0.05f, Vector3.zero))
        {
            Vector3 dir = (targetPosition - transform.position).normalized;

            transform.position += dir * Time.fixedDeltaTime * moveSpeed;
        }
        else
        {
            if (movementType == E_MovementType.EnableMovementReversable)
            {
                if (targetPosition == startPosition)
                {
                    targetPosition = endPosition;
                }
                else if (targetPosition == endPosition)
                {
                    targetPosition = startPosition;
                }
            }
        }
    }

    void MoveInDirection()
    {
        Vector3 dir = transform.rotation * targetPosition;
        transform.position += dir * Time.fixedDeltaTime * moveSpeed;
        //rb.velocity = dir * moveSpeed;
        //rb.velocity = transform.forward * moveSpeed;
        //rb.AddForce(transform.forward * moveSpeed * Time.deltaTime);
    }

    Vector3 GetMovePosition(Vector3 direction)
    {
        Vector3 movePos = RotatePointAroundPivot(direction, transform.position, transform.eulerAngles);

        return movePos;
    }

    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler(angles) * dir;
        point = dir + pivot;
        return point;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.2f);

        Matrix4x4 rotationMatrix = transform.localToWorldMatrix;
        Gizmos.matrix = rotationMatrix;
        Debug.Log(transform.position + " || " + transform.position + endOffset);
        Gizmos.DrawWireSphere(transform.position + endOffset, 0.2f);
    }
}