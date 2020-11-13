using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private const float kPHYSICS_SKIIN_DEPTH = 0.1f;
    
    // Physics and Collisions
    private Vector2 m_velocity = Vector2.zero;
    private Vector2 m_inputTargetVelocity = Vector2.zero;
    private Collider2D m_collider = null;
    private bool m_isCollisionUp, m_isCollisionDown, m_isCollisionRight, m_isCollisionLeft;
    [SerializeField]
    private LayerMask m_worldLayerMask = new LayerMask();


    [Header("Moement Variables")]
    [SerializeField]
    private float m_maxInputMovementSpeed = 1.0f;


    protected void Awake()
    {
        m_collider = this.GetComponent<Collider2D>();
    }

    protected void FixedUpdate()
    {
        ResetFlags();

        m_velocity.x = m_inputTargetVelocity.x;
        Vector2 currentPosition = transform.position;
        Vector2 resolvedPosition = ResolveCollision(currentPosition, ref m_velocity);
        transform.Translate(resolvedPosition - currentPosition);

        m_velocity.y += Physics2D.gravity.y * Time.fixedDeltaTime;
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
        Vector2 newPosition = currentPosition + (currentVelocity * Time.fixedDeltaTime);
        Vector2 bottomCenter = currentPosition + new Vector2(0, -m_collider.bounds.size.y/2);


        float timeLeftAfterCollisionResolve = Time.fixedDeltaTime;
        if(currentVelocity.y <= 0)
        {
            float ySpeed = -currentVelocity.y;
            Vector2 origin = bottomCenter + (Vector2.up*kPHYSICS_SKIIN_DEPTH);
            float distanceMovedThisTick = ySpeed * Time.fixedDeltaTime;
            // Raycast from middle center to check if on grounud
            RaycastHit2D hit = Physics2D.Raycast(bottomCenter + (Vector2.up*kPHYSICS_SKIIN_DEPTH), Vector2.down, distanceMovedThisTick + kPHYSICS_SKIIN_DEPTH, m_worldLayerMask );

            if(hit.collider != null)
            {
                Debug.DrawLine(origin, hit.point, Color.red, Time.fixedDeltaTime);
                m_isCollisionDown = true;
                // Figure out at what point in the frame did the collision occour.
                float t = (hit.distance - kPHYSICS_SKIIN_DEPTH) / distanceMovedThisTick;

                newPosition = currentPosition + ((t * Time.fixedDeltaTime) * currentVelocity);
                currentVelocity.y = Mathf.Max(0, currentVelocity.y);
                timeLeftAfterCollisionResolve -= t * Time.fixedDeltaTime;
            }
            else
            {
                Debug.DrawLine(origin, origin + Vector2.down * (distanceMovedThisTick + kPHYSICS_SKIIN_DEPTH), Color.blue, Time.fixedDeltaTime);
            }
        }

        newPosition += currentVelocity * timeLeftAfterCollisionResolve;

        return newPosition;
    }

}
