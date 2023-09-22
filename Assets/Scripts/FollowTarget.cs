using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform follow;

    private void Start()
    {
        transform.parent = null;
    }

    private void FixedUpdate()
    {
        transform.position = follow.position;
    }
}
