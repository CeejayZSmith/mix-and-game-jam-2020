using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private Vector2 m_velocity = Vector2.zero;
    private Vector2 m_inputTargetVelocity = Vector2.zero;
    private Collider2D m_collider = null;
    [SerializeField]
    private LayerMask m_worldLayerMask = new LayerMask();
    
    [Header("Moement Variables")]
    [SerializeField]
    private float m_maxInputMovementSpeed = 1.0f;

    private bool m_isCollisionUp, m_isCollisionDown, m_isCollisionRight, m_isCollisionLeft;
    private const float kPHYSICS_SKIIN_DEPTH = 0.1f;

    protected void Awake()
    {
        m_collider = this.GetComponent<Collider2D>();
    }

    protected void FixedUpdate()
    {
        m_velocity = m_inputTargetVelocity;
        Vector2 currentPosition = transform.position;
        Vector2 resolvedPosition = ResolveCollision(currentPosition, ref m_velocity);
        transform.Translate(resolvedPosition - currentPosition);
    }

    public void SetDesiredVelocityDirection(Vector2 input)
    {
        if(input.sqrMagnitude > 1.0f)
        {
            input.Normalize();
            Debug.LogWarning("Input velocity directioon to character controller has a magnitude above 1. Had to normalize. ");
        }

        m_inputTargetVelocity.x = input.x * m_maxInputMovementSpeed; 

    }

    protected void ResetFlags()
    {
        m_isCollisionDown = false;
        m_isCollisionLeft = false;
        m_isCollisionRight = false;
        m_isCollisionUp = false;
    }

    protected Vector2 ResolveCollision(Vector2 currentPosition, ref Vector2 currentVelocity)
    {
        return currentPosition + (currentVelocity * Time.fixedDeltaTime);
    }

}
