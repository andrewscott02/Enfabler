using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public bool sprinting { get; protected set; } = false;
    protected float currentSpeed = 0;

    public FootStepData stepData;

    public void SpawnFootstep(int footTransformIndex)
    {
        Instantiate(stepData.footstepObject, stepData.footstepTransforms[footTransformIndex].position, new Quaternion(0, 0, 0, 0));

        SpawnImpulse((sprinting ? stepData.impulseSprintMultiplier : stepData.impulseWalkMultiplier) * currentSpeed);
    }

    void SpawnImpulse(float impulseStrength)
    {
        stepData.impulseSource.GenerateImpulseWithForce(impulseStrength);
    }

    public void DodgeRollLand(float impulseStrength)
    {
        SpawnImpulse(impulseStrength);
        RumbleManager.instance.ControllerRumble(0.25f, 1f, 0.25f);
    }
}
