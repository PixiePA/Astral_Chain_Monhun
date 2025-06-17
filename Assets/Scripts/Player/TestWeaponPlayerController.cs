using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestWeaponPlayerController : PlayerController
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnAttack1Activated(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            animator.SetTrigger("Attack1");
        }
        if (context.performed)
        {
            animator.SetTrigger("HoldAttack1");
        }
    }
}
