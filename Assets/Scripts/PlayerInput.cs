﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public CharacterController m_characterController = null;

    protected void Update()
    {
        Vector2 movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if(movementInput.sqrMagnitude > 1.0f)
        {
            movementInput.Normalize();
        }

        if(m_characterController == null || m_characterController.m_dead == true)
        {
            return;
        }

        if(Input.GetButton("Jump")== true)
        {
            m_characterController.AttemptJump();
        }

        m_characterController.SetDesiredVelocityDirection(movementInput);
    }
}
