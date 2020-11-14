using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public bool m_dead = false;
    private const float kPHYSICS_SKIIN_DEPTH = 0.03f;

    private const int kCOLLISION_SIDE_ITERATIONS = 5;
    [SerializeField]
    private float m_minJumpHeight = 2.0f;
    private float m_timeLeftGround = 0.0f;
    [SerializeField]
    private float m_recoveryJumpTime = 0.1f;
    private bool m_isJumping = false;
    // Physics and Collisions
    private Vector2 m_velocity = Vector2.zero;
    public float m_movementDampingInAir = 1.0f;
    private float m_verticalAccelerationSmoothDamp = 0.0f; 
    private Vector2 m_inputTargetVelocity = Vector2.zero;
    private Collider2D m_collider = null;
    private List<Collider2D> m_groundColliders = new List<Collider2D>();
    private bool m_wasOnGround = false, m_isOnGround = false;

    private bool m_isCollisionUp, m_isCollisionDown, m_isCollisionRight, m_isCollisionLeft;
    [SerializeField]
    private LayerMask m_worldLayerMask = new LayerMask();


    [Header("Moement Variables")]
    [SerializeField]
    private float m_maxInputMovementSpeed = 1.0f;


    protected void Awake()
    {
        m_collider = this.GetComponent<Collider2D>();
        GameManager.Instance.m_allCharacterControllers.Add(this);
    }


    public void Spawn(Vector2 position)
    {
        transform.position = position;
        m_velocity = Vector2.zero;
        m_dead = false;
        this.gameObject.SetActive(true);
    }

    public void PrepareForPool()
    {
        m_dead = true;
        m_velocity = Vector2.zero;
        this.gameObject.SetActive(false);
    }
    protected void FixedUpdate()
    {
        m_wasOnGround = m_isOnGround;
        ResetFlags();

        if(m_wasOnGround == true)
        {
            m_velocity.x = m_inputTargetVelocity.x;
            m_verticalAccelerationSmoothDamp = 0;
        }
        else
        {
            m_velocity.x = Mathf.SmoothDamp(m_velocity.x, m_inputTargetVelocity.x, ref m_verticalAccelerationSmoothDamp, m_movementDampingInAir);
        }


        Vector2 currentPosition = transform.position;
        Vector2 resolvedPosition = ResolveCollision(currentPosition, ref m_velocity);

        if(m_wasOnGround == false && m_isOnGround == true)
        {
            OnLanded();
        }

        if(m_wasOnGround == true && m_isOnGround == false)
        {
            m_timeLeftGround = Time.time;
        }

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
        if((m_isOnGround == false && m_timeLeftGround + m_recoveryJumpTime < Time.time) || m_isJumping == true)
        {                
            return;
        }
        OnJumpBegin();
        Jump();
    }
    private void Jump()
    {
        m_timeLeftGround = Time.time;
        m_isOnGround = false;
        m_isCollisionDown = false;
        m_isJumping = true;

        float impulseVelocity = JumpVelocity();
        m_velocity.y = Mathf.Max(m_velocity.y, impulseVelocity);
        OnJumpBegin();
    }

    private float JumpVelocity()
    {
        return Mathf.Sqrt(-(2*Physics2D.gravity.y*m_minJumpHeight));
    }

    private void OnLanded()
    {
        foreach(Collider2D collider in m_groundColliders)
        {
            Tile tile = collider.GetComponent<Tile>();

            if(tile != null)
            {
                tile.OnPlayerLandedOnTile();
            }
        }
        m_isJumping = false;
    }

    protected void ResetFlags()
    {
        m_isCollisionDown = false;
        m_isCollisionLeft = false;
        m_isCollisionRight = false;
        m_isCollisionUp = false;
        m_isOnGround = false;
        m_groundColliders.Clear();
    }

    private bool CheckSideCollision(Vector2 sideStart, Vector2 sideEnd, Vector2 sideNormal, float distanceMoved, out float timeOfCollision)
    {
        List<Collider2D> temp = new List<Collider2D>();
        return CheckSideCollision(sideStart, sideEnd, sideNormal, distanceMoved, out timeOfCollision, ref temp);
    }
    private bool CheckSideCollision(Vector2 sideStart, Vector2 sideEnd, Vector2 sideNormal, float distanceMoved, out float timeOfCollision, ref List<Collider2D> collidedWith)
    {
        timeOfCollision = 0;
        if(distanceMoved == 0)
        {
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
                // -0.001f is to prevent the collider edges froming being in the exact same position when resolved.
                // this can cause issues with vertical raycasts hiting walls, and horizontal hitting floors, as they are on the same plane
                // If this causes pixel gap issues we could resolve this using using a dot product of the hit normal and side normal to return fales on the collision as we have no slops to account for.
                float t = (hit.distance - kPHYSICS_SKIIN_DEPTH - 0.001f) / distanceMoved;
                timeOfCollision = Mathf.Max(0, t);
                if(collidedWith.Contains(hit.collider) == false)
                {
                    collidedWith.Add(hit.collider);
                }
            }
            else
            {
                Debug.DrawLine(origin, origin + (sideNormal * (distanceMoved +kPHYSICS_SKIIN_DEPTH )), Color.blue, Time.fixedDeltaTime);
            }
        }
        return collidedWith.Count > 0;
    }

    protected Vector2 ResolveCollision(Vector2 currentPosition, ref Vector2 currentVelocity)
    {
        Vector2 preVelocity = currentVelocity;
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
                    newPosition += currentVelocity * Mathf.Max(0, (timeOfHorizontalCollision * remainingTime));
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

                if(m_isCollisionDown || m_isCollisionUp)
                {
                    newPosition += currentVelocity * Mathf.Max(0, (timeOfVerticalCollision * remainingTime));
                }
                else
                {
                    newPosition += currentVelocity * remainingTime;
                }
            }
        }
        m_groundColliders.Clear();
        m_isOnGround = CheckSideCollision(bottomLeftOffset + newPosition, bottomRightOffset + newPosition, Vector2.down, 0.001f, out timeOfVerticalCollision, ref m_groundColliders);
        if(m_groundColliders.Count > 0)
        {
            foreach(Collider2D collider in m_groundColliders)
            {
                CoinTileModule tile = collider.GetComponent<CoinTileModule>();
                if(tile != null)
                {
                    // auto jump on coin tiles
                    currentVelocity.y = JumpVelocity();
                    break;
                }
            }
        }
        return newPosition;
    }

}
