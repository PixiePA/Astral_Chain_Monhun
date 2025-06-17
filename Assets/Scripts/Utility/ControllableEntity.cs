using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(PlayerInput))]
public class ControllableEntity : MonoBehaviour
{
    //Inspector Settings
    public float walkSpeed = 1f;

    public float runSpeed = 3f;

    [Range(0f, 1f)]
    public float runThreshold = 0.3f;

    [SerializeField]
    [Range(0f, 1f)]
    protected float inputSpeed;

    [SerializeField]
    [Range(0f, 1f)]
    protected float speedToTargetSpeedLerpRate = 0.3f;

    [SerializeField]
    [Range(0f, 1f)]
    protected float rotateToTargetSpeedLerpRate = 0.3f;

    [SerializeField]
    [Range(-1f, 1f)]
    protected float lean;

    [SerializeField]
    protected float maxLeanTurnRate = 15f;

    [SerializeField]
    protected Animator animator;

    [SerializeField]
    protected GameObject entityCharacter;

    [SerializeField]
    protected Rigidbody entityRb;


    //Behind the scenes values
    protected Vector2 moveInputValue;
    protected Vector2 rawMoveInputValue;
    protected float targetSpeed;

    protected Vector2 CurrentMoveDirection
    {
        get
        {
            return new Vector2(entityCharacter.transform.forward.x, entityCharacter.transform.forward.z).normalized;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Awake()
    {
        if (!animator)
        {
            animator = GetComponent<Animator>();
        }
    }

    protected float GetRotationFromDirection(Vector2 direction)
    {
        return Mathf.Rad2Deg * GetRadiansFromDirection(direction);
    }

    protected float GetRadiansFromDirection(Vector2 direction)
    {
        return Mathf.Atan2(direction.x, direction.y);
    }

    protected Vector2 RotateVector2AroundRadians(Vector2 direction, float radians)
    {
        // Rotate angle by given radians
        return new Vector2(
            direction.x * Mathf.Cos(radians) - direction.y * Mathf.Sin(radians),
            direction.x * Mathf.Sin(radians) + direction.y * Mathf.Cos(radians)
            );
    }

    protected virtual void UpdateEntityVelocity()
    {
        // Use speed to calculate the desired velocity of the player
        float trueSpeed;
        if (inputSpeed < runThreshold)
        {
            trueSpeed = Mathf.Lerp(0f, walkSpeed, inputSpeed / runThreshold);
        }
        else
        {
            trueSpeed = Mathf.Lerp(walkSpeed, runSpeed, (inputSpeed - runThreshold) / (1 - runThreshold));
        }

        animator.SetFloat("Speed", trueSpeed);

        Vector3 desiredHorizontalVelocity = entityCharacter.transform.forward * trueSpeed;
        Vector3 velocityDifference = desiredHorizontalVelocity - entityRb.linearVelocity;
        Vector3 horizontalVelocityDifference = new Vector3(velocityDifference.x, 0, velocityDifference.z);

        entityRb.AddForce(horizontalVelocityDifference, ForceMode.VelocityChange);
    }

    protected virtual void LerpSpeedAndRotation()
    {
        //Lerping speed
        LerpSpeed();

        //Lerping rotation
        LerpRotation();
    }
    
    protected virtual void LerpSpeed()
    {
        if (Mathf.Abs(inputSpeed - targetSpeed) < 0.1)
        {
            inputSpeed = targetSpeed;
        }
        else
        {
            inputSpeed = Mathf.Lerp(inputSpeed, targetSpeed, speedToTargetSpeedLerpRate);
        }
    }

    protected virtual void LerpRotation()
    {
        if (moveInputValue.sqrMagnitude > 0)
        {
            float currentRotationInDegrees = GetRotationFromDirection(CurrentMoveDirection);
            float targetRotationInDegrees = GetRotationFromDirection(moveInputValue.normalized);
            float rotationDifference = targetRotationInDegrees - currentRotationInDegrees;
            if (rotationDifference > 180)
            {
                rotationDifference -= 360;
            }
            else if (rotationDifference < -180)
            {
                rotationDifference += 360;
            }
            float newRotation = currentRotationInDegrees + rotationDifference * rotateToTargetSpeedLerpRate;
            entityCharacter.transform.localRotation = Quaternion.Euler(new Vector3(0, newRotation, 0));

            lean = Mathf.Clamp(rotationDifference / maxLeanTurnRate, -1, 1);

            float animatorLean = Mathf.Lerp(animator.GetFloat("Lean"), lean, rotateToTargetSpeedLerpRate);
            animator.SetFloat("Lean", animatorLean);
        }
    }

    protected virtual void UpdateEntityMovement()
    {
        LerpSpeedAndRotation();

        // If close enough to ground, snap
        if (Physics.Raycast(entityCharacter.transform.position, Vector3.down, out RaycastHit hitInfo, 0.3f, 8))
        {
            entityRb.AddForce(-hitInfo.normal * 100, ForceMode.Acceleration);
            entityRb.useGravity = false;
        }
        else
        {
            entityRb.useGravity = true;
        }

        UpdateEntityVelocity();

    }
}
