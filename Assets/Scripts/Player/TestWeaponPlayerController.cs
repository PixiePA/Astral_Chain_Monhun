using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class TestWeaponPlayerController : PlayerController
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void OnBufferEnded()
    {
        
    }

    public void OnAttack1Activated(InputAction.CallbackContext context)
    {
        switch (state)
        {
            case "Punch":
                if (context.canceled)
                {
                    animator.SetTrigger("Attack1");
                }
                else if (context.performed)
                {
                    animator.SetTrigger("HoldAttack1");
                }
                break;
            case "Elbow":
                if (context.performed)
                {
                    animator.SetTrigger("HoldAttack1");
                }
                break;
            case "Hook":
                if (context.started)
                {
                    if (inputBuffer <= 0)
                    {
                        attack1Pressed = true;
                        inputBuffer = 0.3f;
                    }
                    else
                    {
                        if (attack2Pressed)
                        {
                            animator.SetTrigger("Attack1+Attack2");
                        }
                    }
                }
                break;
            case "CrossPunch":
                break;
            default:
                if (context.started)
                {
                    animator.SetTrigger("Attack1");
                }
                break;

        }
        
    }

    public void OnAttack2Activated(InputAction.CallbackContext context)
    {
        switch (state)
        {
            case "Hook":
                if (context.started)
                {
                    if (inputBuffer <= 0)
                    {
                        attack2Pressed = true;
                        inputBuffer = 0.3f;
                    }
                    else
                    {
                        if (attack1Pressed)
                        {
                            animator.SetTrigger("Attack1+Attack2");
                        }
                    }
                }
                break;
        }
    }

}
