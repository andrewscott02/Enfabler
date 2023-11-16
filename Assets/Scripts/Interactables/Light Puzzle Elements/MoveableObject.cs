using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableObject : PuzzleElement
{
    public E_MovementType movementType;
    public float moveDelay = 1f;
    public float moveSpeed = 5f;
    public Vector3 endOffset;

    Vector3 startPosition, endPosition;
    Vector3 targetPosition;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        startPosition = transform.position;
        endPosition = startPosition + endOffset;

        switch (movementType)
        {
            case E_MovementType.Toggle:
                moving = true;
                StartCoroutine(IMoveToPosition(startPosition, moveDelay));
                break;
            case E_MovementType.EnableMovement:
                moving = false;
                targetPosition = startPosition;
                break;
        }
    }

    protected override void Activate()
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
        }
    }

    protected override void Deactivate()
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

    private void Update()
    {
        if (!moving) return;

        if (!HelperFunctions.AlmostEqualVector3(targetPosition, transform.position, 0.05f, Vector3.zero))
        {
            Vector3 dir = (targetPosition - transform.position).normalized;

            transform.position += dir * Time.deltaTime * moveSpeed;
        }
        else
        {
            if (movementType == E_MovementType.EnableMovement)
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.2f);
        Gizmos.DrawWireSphere(transform.position + endOffset, 0.2f);
    }
}

public enum E_MovementType
{
    Toggle, EnableMovement
}