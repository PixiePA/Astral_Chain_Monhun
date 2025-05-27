using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

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
    private float speedToTargetSpeedLerpRate = 0.3f;

    [SerializeField][Range(-1f, 1f)]
    private float lean;

    [SerializeField]
    private bool isSprinting;

    [SerializeField] 
    private Animator animator;

    [SerializeField]
    private GameObject playerCharacter;

    [SerializeField]
    private GameObject playerCamera;


    //Behind the scenes values
    private Vector2 moveInputValue;
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
        speed = Mathf.Lerp(speed, targetSpeed, speedToTargetSpeedLerpRate);
        Debug.Log(targetSpeed);

        animator.SetFloat("Speed", speed);
    }

    public void OnMove(InputValue value)
    {
        Vector2 newMoveValue = value.Get<Vector2>();
        lean = newMoveValue.x;
        moveInputValue = newMoveValue;
        targetSpeed = moveInputValue.sqrMagnitude;

        //Cap speed if is not sprinting
        if (!isSprinting)
        {
            targetSpeed = Mathf.Min(targetSpeed, sprintSpeed);
        }

        if (targetSpeed < sprintSpeed)
        {
            isSprinting = false;
        }

        animator.SetFloat("Lean", lean);
    }

    public void OnSprint(InputValue value)
    {
        isSprinting = !isSprinting;
    }

    private float GetRotationFromDirection(Vector2 direction)
    {
        throw new NotImplementedException();
    }

    
}
