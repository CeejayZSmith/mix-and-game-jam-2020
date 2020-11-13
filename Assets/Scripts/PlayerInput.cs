using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public CharacterController m_characterController = null;

    protected void Update()
    {
        Vector2 movementInput = new Vector2(Input.GetAxisRaw("HorizontalMovement"), Input.GetAxisRaw("VerticalMovement"));
        Debug.Log(movementInput);
        if(movementInput.sqrMagnitude > 1.0f)
        {
            movementInput.Normalize();
        }

        m_characterController.SetDesiredVelocityDirection(movementInput);
    }
}
