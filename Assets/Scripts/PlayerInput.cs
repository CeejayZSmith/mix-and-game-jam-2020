using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public CharacterController m_characterController = null;

    protected void Update()
    {
        Vector2 movementInput = new Vector2(Input.GetAxisRaw("HorizontalMovement"), Input.GetAxisRaw("VerticalMovement"));
        if(movementInput.sqrMagnitude > 1.0f)
        {
            movementInput.Normalize();
        }

        if(Input.GetButton("Jump")== true)
        {
            m_characterController.AttemptJump();
        }

        m_characterController.SetDesiredVelocityDirection(movementInput);
    }
}
