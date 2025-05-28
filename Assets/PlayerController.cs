using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    //Inspector Settings
    public float walkSpeed = 1f;

    public float runSpeed = 3f;

    [Range(0f, 1f)]
    public float runThreshold = 0.3f;

    [SerializeField][Range(0f, 1f)]
    private float speed;

    [SerializeField][Range(0f, 1f)]
    private float sprintSpeed = 0.6f;

    [SerializeField][Range(0f, 1f)]
    private float sprintStopSpeed = 0.6f;

    [SerializeField][Range(0f, 1f)]
    private float speedToTargetSpeedLerpRate = 0.3f;

    [SerializeField]
    [Range(0f, 1f)]
    private float rotateToTargetSpeedLerpRate = 0.3f;

    [SerializeField][Range(-1f, 1f)]
    private float lean;

    [SerializeField]
    private float maxLeanTurnRate = 15f;

    [SerializeField]
    private bool isSprinting;

    [SerializeField] 
    private Animator animator;

    [SerializeField]
    private GameObject playerCharacter;

    [SerializeField]
    private GameObject playerCamera;

    [SerializeField]
    private Rigidbody playerRb;


    //Behind the scenes values
    private Vector2 moveInputValue;
    private Vector2 rawMoveInputValue;
    private float targetSpeed;

    private Vector2 CurrentMoveDirection
    {
        get
        {
            return new Vector2(playerCharacter.transform.forward.x, playerCharacter.transform.forward.z).normalized;
        }
    }

    private Vector2 CurrentCameraDirection
    {
        get
        {
            return new Vector2(playerCamera.transform.forward.x, playerCamera.transform.forward.z).normalized;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (!animator)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        UpdateMoveInput();

        //Lerping speed
        if (Mathf.Abs(speed - targetSpeed) < 0.1)
        {
            speed = targetSpeed;
        }
        else
        {
            speed = Mathf.Lerp(speed, targetSpeed, speedToTargetSpeedLerpRate);
        }

        //Lerping rotation
        if (moveInputValue.magnitude > 0)
        {
            float currentRotationInDegrees = GetRotationFromDirection(CurrentMoveDirection);
            float targetRotationInDegrees = GetRotationFromDirection(moveInputValue.normalized);
            float rotationDifference  = targetRotationInDegrees - currentRotationInDegrees;
            if (rotationDifference > 180) 
            {
                rotationDifference -= 360;
            }
            else if (rotationDifference < -180)
            {
                rotationDifference += 360;
            }
            float newRotation = currentRotationInDegrees + rotationDifference * rotateToTargetSpeedLerpRate;
            playerCharacter.transform.localRotation = Quaternion.Euler(new Vector3(0, newRotation, 0));

            lean = Mathf.Clamp(rotationDifference/maxLeanTurnRate, -1, 1);

            float animatorLean = Mathf.Lerp(animator.GetFloat("Lean"), lean, rotateToTargetSpeedLerpRate);
            animator.SetFloat("Lean", animatorLean);
        }

        float trueSpeed;
        if (speed < runThreshold)
        {
            trueSpeed = Mathf.Lerp(0f, walkSpeed, speed / runThreshold);
        }
        else
        {
            trueSpeed = Mathf.Lerp(walkSpeed, runSpeed, (speed - runThreshold) / (1 - runThreshold));
        }

        animator.SetFloat("Speed", trueSpeed);

        Vector3 desiredHorizontalVelocity = playerCharacter.transform.forward * trueSpeed;
        Vector3 velocityDifference = desiredHorizontalVelocity - playerRb.linearVelocity;
        Vector3 horizontalVelocityDifference = new Vector3(velocityDifference.x, 0, velocityDifference.z);

        playerRb.AddForce(horizontalVelocityDifference, ForceMode.VelocityChange);
    }

    public void OnMove(InputValue value)
    {
        rawMoveInputValue = value.Get<Vector2>();
        UpdateMoveInput();
    }

    public void OnLook(InputValue value)
    {
        UpdateMoveInput();
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = !isSprinting;
    }

    private float GetRotationFromDirection(Vector2 direction)
    {
        return Mathf.Rad2Deg*GetRadiansFromDirection(direction);
    }

    private float GetRadiansFromDirection(Vector2 direction)
    {
        return Mathf.Atan2(direction.x, direction.y);
    }

    private Vector2 RotateVector2AroundRadians(Vector2 direction, float radians)
    {
        // Rotate angle by given radians
        return new Vector2(
            direction.x * Mathf.Cos(radians) - direction.y * Mathf.Sin(radians), 
            direction.x * Mathf.Sin(radians) + direction.y * Mathf.Cos(radians)
            );
    }

    private void UpdateMoveInput()
    {
        Vector2 newMoveValue = RotateVector2AroundRadians(rawMoveInputValue, -GetRadiansFromDirection(CurrentCameraDirection));
        moveInputValue = newMoveValue;
        targetSpeed = moveInputValue.sqrMagnitude;

        //Cap speed if is not sprinting
        if (!isSprinting)
        {
            targetSpeed = Mathf.Min(targetSpeed, sprintSpeed);
        }

        if (targetSpeed < sprintStopSpeed)
        {
            isSprinting = false;
        }
    }
    
}
