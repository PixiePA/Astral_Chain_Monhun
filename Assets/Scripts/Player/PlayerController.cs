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

    protected float attack1HeldTime;

    protected int attack1PressCount;

    protected float attack1TimeSinceLastPress;

    protected bool attack2Pressed;

    protected float attack2HeldTime;

    protected int attack2PressCount;

    protected float attack2TimeSinceLastPress;

    protected bool attack3Pressed;

    protected float attack3HeldTime;

    protected int attack3PressCount;

    protected float attack3TimeSinceLastPress;

    protected float inputBuffer;

    protected bool inputBufferActivated;

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
    protected virtual void Update()
    {
        if (attack1Pressed)
        {
            attack1HeldTime += Time.deltaTime;
        }

        if (attack1PressCount > 0)
        {
            attack1TimeSinceLastPress += Time.deltaTime;
        }

        if (attack2Pressed)
        {
            attack2HeldTime += Time.deltaTime;
        }
        if (attack2PressCount > 0)
        {
            attack2TimeSinceLastPress += Time.deltaTime;
        }

        if (attack3Pressed)
        {
            attack3HeldTime += Time.deltaTime;
        }
        if (attack3PressCount > 0)
        {
            attack3TimeSinceLastPress += Time.deltaTime;
        }

        if (inputBufferActivated)
        {
            inputBuffer += Time.deltaTime;
        }

        UpdateAnimator();

    }

    protected virtual void OnBufferEnded()
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

    public void UpdateAnimator()
    {
        animator.SetBool("Attack1Pressed", attack1Pressed);
        animator.SetFloat("Attack1HeldTime", attack1HeldTime);
        animator.SetInteger("Attack1PressCount", attack1PressCount);
        animator.SetFloat("Attack1TimeSinceLastPress", attack1TimeSinceLastPress);
        animator.SetBool("Attack2Pressed", attack2Pressed);
        animator.SetFloat("Attack2HeldTime", attack2HeldTime);
        animator.SetInteger("Attack2PressCount", attack2PressCount);
        animator.SetFloat("Attack2TimeSinceLastPress", attack2TimeSinceLastPress);
        animator.SetBool("Attack3Pressed", attack3Pressed);
        animator.SetFloat("Attack3HeldTime", attack3HeldTime);
        animator.SetInteger("Attack3PressCount", attack3PressCount);
        animator.SetFloat("Attack3TimeSinceLastPress", attack3TimeSinceLastPress);
        animator.SetFloat("InputBuffer", inputBuffer);
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

    protected void ResetAttackInputs(Animator animator)
    {
        if (this.animator == animator)
        {
            ResetAttackInputs();
        }
    }

    protected void ResetAttackInputs()
    {
        inputBuffer = 0;
        inputBufferActivated = false;
        attack1Pressed = false;
        attack1TimeSinceLastPress = 0;
        attack1PressCount = 0;
        attack1HeldTime = 0;
        attack2Pressed = false;
        attack2TimeSinceLastPress = 0;
        attack2PressCount = 0;  
        attack2HeldTime = 0;
        attack3Pressed = false;
        attack3TimeSinceLastPress = 0;
        attack3PressCount = 0;
        attack3HeldTime = 0;

        UpdateAnimator();
    }


    private void ChangeCanMove(bool canMove, Animator animator)
    {
        if (this.animator == animator)
        {
            this.canMove = canMove;
        }

    }

    private void ChangeState(string newState, Animator animator)
    {
        if (this.animator == animator)
        {
            state = newState;
        }

    }

    private void ChangeCanTurn(bool canTurn, Animator animator)
    {
        if (this.animator == animator)
        {
            this.canTurn = canTurn;
        }
    }

    public void OnAttack1Activated(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            attack1PressCount++;
            attack1TimeSinceLastPress = 0;
            attack1HeldTime = 0;
            attack1Pressed = true;
            inputBufferActivated = true;
            UpdateAnimator();
        }
        if (context.canceled)
        {
            attack1Pressed = false;
            UpdateAnimator();
        }

    }

    public void OnAttack2Activated(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            attack2PressCount++;
            attack2TimeSinceLastPress = 0;
            attack2HeldTime = 0;
            attack2Pressed = true;
            inputBufferActivated = true;
            UpdateAnimator();
        }
        if (context.canceled)
        {
            attack2Pressed = false;
            UpdateAnimator();
        }
    }

    public void OnAttack3Activated(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            attack3PressCount++;
            attack3TimeSinceLastPress = 0;
            attack3HeldTime = 0;
            attack3Pressed = true;
            inputBufferActivated = true;
            UpdateAnimator();
        }
        if (context.canceled)
        {
            attack3Pressed = false;
            UpdateAnimator();
        }
    }
}
