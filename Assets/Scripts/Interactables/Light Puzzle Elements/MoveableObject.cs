using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableObject : PuzzleElement
{
    public E_MovementType movementType;
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

        MoveToPosition(startPosition);
    }

    protected override void Activate()
    {
        Debug.Log("Unlock Interaction");

        switch (movementType)
        {
            case E_MovementType.Toggle:
                MoveToPosition(endPosition);
                break;
        }
    }

    protected override void Deactivate()
    {
        Debug.Log("Unlock Interaction");

        switch (movementType)
        {
            case E_MovementType.Toggle:
                MoveToPosition(startPosition);
                break;
        }
    }

    void MoveToPosition(Vector3 position)
    {
        targetPosition = position;
    }

    void EnableMovement(bool enabled)
    {

    }

    private void Update()
    {
        if (!HelperFunctions.AlmostEqualVector3(targetPosition, transform.position, 0.05f, Vector3.zero))
        {
            Vector3 dir = (targetPosition - transform.position).normalized;

            transform.position += dir * Time.deltaTime * moveSpeed;
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