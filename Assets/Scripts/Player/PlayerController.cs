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
    protected float sprintSpeed = 0.6f;

    [SerializeField]
    [Range(0f, 1f)]
    protected float sprintStopSpeed = 0.6f;

    [SerializeField]
    protected bool isSprinting;

    [SerializeField]
    protected GameObject playerCamera;

    protected bool canMove;

    protected bool canTurn;

    protected bool attack1Pressed;

    protected bool attack1Held;

    protected bool attack2Pressed;

    protected bool attack2Held;

    protected bool attack3Pressed;

    protected bool attack3Held;

    protected float inputBuffer;

    protected string state;

    protected Vector2 CurrentCameraDirection
    {
        get
        {
            return new Vector2(playerCamera.transform.forward.x, playerCamera.transform.forward.z).normalized;
        }
    }

    private void OnEnable()
    {
        PlayerEvents.onChangeCanMove += ChangeCanMove;
        PlayerEvents.onChangeCanTurn += ChangeCanTurn;
        PlayerEvents.onChangeState += ChangeState;
        PlayerEvents.onResetAttackInputs += ResetAttackInputs;
    }

    private void OnDisable()
    {
        PlayerEvents.onChangeCanMove -= ChangeCanMove;
        PlayerEvents.onChangeCanTurn -= ChangeCanTurn;
        PlayerEvents.onChangeState -= ChangeState;
        PlayerEvents.onResetAttackInputs -= ResetAttackInputs;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void FixedUpdate()
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

    protected void UpdateMoveInput()
    {
        Vector2 newMoveValue = RotateVector2AroundRadians(rawMoveInputValue, -GetRadiansFromDirection(CurrentCameraDirection));
        moveInputValue = newMoveValue;

        if (canMove)
        {
            targetSpeed = moveInputValue.magnitude;
        }
        else
        {
            targetSpeed = 0;
        }
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

    protected override void LerpRotation()
    {
        if (canTurn)
        {
            base.LerpRotation();
        }
    }

    protected void ResetAttackInputs()
    {
        inputBuffer = 0;
        attack1Held = false;
        attack2Held = false;   
        attack3Held = false;
        attack1Pressed = false;
        attack2Pressed = false;
        attack3Pressed = false;
    }


    private void ChangeCanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    private void ChangeState(string newState)
    {
        state = newState;
    }

    private void ChangeCanTurn(bool canTurn)
    {
        this.canTurn = canTurn;
    }
}
