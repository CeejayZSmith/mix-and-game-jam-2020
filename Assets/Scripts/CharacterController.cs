using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private const float kPHYSICS_SKIIN_DEPTH = 0.1f;
    private const int kCOLLISION_SIDE_ITERATIONS = 3;
    [SerializeField]
    private float m_minJumpHeight = 2.0f;
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

        if(m_isCollisionDown == true)
        {
            m_velocity.y = Mathf.Max(0, m_velocity.y);
        }
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

    private void OnJumpBegin()
    {

    }

    public void AttemptJump()
    {
        if(m_isCollisionDown == false)
        {                
            return;
        }
        Jump();
    }
    private void Jump()
    {

        m_isCollisionDown = false;

        float impulseVelocity = Mathf.Sqrt(-(2*Physics2D.gravity.y*m_minJumpHeight));
        m_velocity.y = Mathf.Max(m_velocity.y, impulseVelocity);
        OnJumpBegin();
    }

    private void OnJumpFinished()
    {

    }

    protected void ResetFlags()
    {
        m_isCollisionDown = false;
        m_isCollisionLeft = false;
        m_isCollisionRight = false;
        m_isCollisionUp = false;
    }

    private bool CheckSideCollision(Vector2 sideStart, Vector2 sideEnd, Vector2 sideNormal, float distanceMoved, out float timeOfCollision)
    {
        if(distanceMoved == 0)
        {
            timeOfCollision = 0;
            return false;
        }

        Vector2 sideDiff = sideEnd - sideStart;
        for(int i = 0; i < kCOLLISION_SIDE_ITERATIONS; i++)
        {

            Vector2 origin = sideStart + (i*(sideDiff/(kCOLLISION_SIDE_ITERATIONS-1))) + (-sideNormal*kPHYSICS_SKIIN_DEPTH);
            // Raycast from middle center to check if on grounud
            RaycastHit2D hit = Physics2D.Raycast(origin, sideNormal, distanceMoved + kPHYSICS_SKIIN_DEPTH, m_worldLayerMask );

            if(hit.collider != null)
            {

                Debug.DrawLine(origin, hit.point, Color.red, Time.fixedDeltaTime);

                // Figure out at what point in the frame did the collision occour.
                float t = (hit.distance - kPHYSICS_SKIIN_DEPTH - 0.01f) / distanceMoved;
                timeOfCollision = Mathf.Max(0, t);
                // No uneven ground or slopes so the first collision will most likely be the only collision, unless are very high speeds.
                // To fix this we can just iterate through every raycast and only set timeOfCollision if its smaller than the current timeOfCollision
                return true;
            }
            else
            {
                Debug.DrawLine(origin, origin + (sideNormal * (distanceMoved +kPHYSICS_SKIIN_DEPTH )), Color.blue, Time.fixedDeltaTime);
            }
        }
        timeOfCollision = 0;
        return false;
    }

    protected Vector2 ResolveCollision(Vector2 currentPosition, ref Vector2 currentVelocity)
    {
        Vector2 newPosition = currentPosition + (currentVelocity * Time.fixedDeltaTime);
        Vector2 bottomLeftOffset = new Vector2(-m_collider.bounds.size.x/2, -m_collider.bounds.size.y/2);
        Vector2 topRightOffset = -bottomLeftOffset;
        Vector2 topLeftOffset = new Vector2(bottomLeftOffset.x, topRightOffset.y);
        Vector2 bottomRightOffset = new Vector2(topRightOffset.x, bottomLeftOffset.y);

        float timeOfVerticalCollision = 0.0f;
        if(currentVelocity.y > 0)
        {
            m_isCollisionUp = CheckSideCollision(topLeftOffset + currentPosition, topRightOffset + currentPosition, Vector2.up, currentVelocity.y * Time.fixedDeltaTime, out timeOfVerticalCollision);
        }
        else
        {
            m_isCollisionDown = CheckSideCollision(bottomLeftOffset + currentPosition, bottomRightOffset + currentPosition, Vector2.down, -currentVelocity.y * Time.fixedDeltaTime, out timeOfVerticalCollision);
        }

        float timeOfHorizontalCollision = 0.0f;
        if(currentVelocity.x > 0)
        {
            m_isCollisionRight = CheckSideCollision(topRightOffset + currentPosition, bottomRightOffset + currentPosition, Vector2.right, currentVelocity.x * Time.fixedDeltaTime, out timeOfHorizontalCollision);
        }
        else
        {
            m_isCollisionLeft = CheckSideCollision(topLeftOffset + currentPosition, bottomLeftOffset + currentPosition, Vector2.left, -currentVelocity.x * Time.fixedDeltaTime, out timeOfHorizontalCollision);
        }

        bool hasHorizontalCollision = m_isCollisionRight || m_isCollisionLeft;
        bool hasVerticalCollision = m_isCollisionUp || m_isCollisionDown;
        bool hasCollision = hasVerticalCollision || hasHorizontalCollision;

        if(hasCollision == false)
        {
            newPosition = currentPosition + (currentVelocity * Time.fixedDeltaTime);
        }
        else
        {
            bool verticalFirst = timeOfVerticalCollision < timeOfHorizontalCollision;

            float remainingTime = Time.fixedDeltaTime;
            if((hasVerticalCollision == true && hasHorizontalCollision == false) || (hasVerticalCollision == true && verticalFirst == true))
            {
                float time = timeOfVerticalCollision * Time.fixedDeltaTime; 
                newPosition = currentPosition + (currentVelocity * time);
                remainingTime -= time;
                
                currentVelocity.y = 0;

                if(currentVelocity.x > 0)
                {
                    m_isCollisionRight = CheckSideCollision(topRightOffset + newPosition, bottomRightOffset + newPosition, Vector2.right, currentVelocity.x * remainingTime, out timeOfHorizontalCollision);
                }
                else
                {
                    m_isCollisionLeft = CheckSideCollision(topLeftOffset + newPosition, bottomLeftOffset + newPosition, Vector2.left, -currentVelocity.x * remainingTime, out timeOfHorizontalCollision);
                }

                if(m_isCollisionLeft || m_isCollisionRight)
                {
                    newPosition += currentVelocity * Mathf.Max(0, ((timeOfHorizontalCollision * remainingTime) - time));
                }
                else
                {
                    newPosition += currentVelocity * remainingTime;
                }

            }
            else
            {
                float time = timeOfHorizontalCollision * Time.fixedDeltaTime; 
                newPosition = currentPosition + (currentVelocity * time);
                remainingTime -= time;
                
                currentVelocity.x = 0;

                timeOfVerticalCollision = 0.0f;
                if(currentVelocity.y > 0)
                {
                    m_isCollisionUp = CheckSideCollision(topLeftOffset + newPosition, topRightOffset + newPosition, Vector2.up, currentVelocity.y * remainingTime, out timeOfVerticalCollision);
                }
                else
                {
                    m_isCollisionDown = CheckSideCollision(bottomLeftOffset + newPosition, bottomRightOffset + newPosition, Vector2.down, -currentVelocity.y * remainingTime, out timeOfVerticalCollision);
                }

                if(m_isCollisionLeft || m_isCollisionRight)
                {
                    newPosition += currentVelocity * Mathf.Max(0, ((timeOfVerticalCollision * remainingTime) - time));
                }
                else
                {
                    newPosition += currentVelocity * remainingTime;
                }
            }
        }
        return newPosition;
    }

}
