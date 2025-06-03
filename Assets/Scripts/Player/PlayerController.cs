using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : ControllableEntity
{
    //Inspector Settings

    [SerializeField]
    [Range(0f, 1f)]
    private float sprintSpeed = 0.6f;

    [SerializeField]
    [Range(0f, 1f)]
    private float sprintStopSpeed = 0.6f;

    [SerializeField]
    private bool isSprinting;

    [SerializeField]
    private GameObject playerCamera;

    private Vector2 CurrentCameraDirection
    {
        get
        {
            return new Vector2(playerCamera.transform.forward.x, playerCamera.transform.forward.z).normalized;
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
        
        UpdateEntityMovement();

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        rawMoveInputValue = context.ReadValue<Vector2>();
        UpdateMoveInput();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        UpdateMoveInput();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.control.device.name == "Keyboard")
        {
            
            if (context.started)
            {
                isSprinting = true;
            }
            else if (context.canceled)
            {
                isSprinting = false;
            }
        }
        else
        {
            if (context.started)
            {
                isSprinting = !isSprinting;
            }
        }
    }

    private void UpdateMoveInput()
    {
        Vector2 newMoveValue = RotateVector2AroundRadians(rawMoveInputValue, -GetRadiansFromDirection(CurrentCameraDirection));
        moveInputValue = newMoveValue;
        targetSpeed = moveInputValue.magnitude;

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
