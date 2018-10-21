using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {


    private bool bIsWalking = false;
    private bool bIsClimbing = false;

    //Movement Variables
    [SerializeField]
    private float m_JogSpeed = 3;
    [SerializeField]
    private float m_WalkSpeed = 1.5f;
    [SerializeField]
    private float m_ClimbSpeed = 1f;

    //For Fine-tuning movement
    [SerializeField]
    private float m_GroundControl = 0.1f;
    [SerializeField]
    private float m_AirControl = 0.02f;
    [SerializeField]
    private float m_ClimbControl = 0.1f;
    [SerializeField]
    private float m_MinAirSpeed = 1;
    [SerializeField]
    private float m_GravityMult = 1;
    [SerializeField]
    private float m_StickToGround = 3;
    [SerializeField]
    private float m_Friction = 5;
    [SerializeField]
    private float surfAngleThreshold = 20f;

    //This variable determines the speed at which player sticks to climbable surfaces
    [SerializeField]
    private AnimationCurve stickToClimbableSpeed;



    //Internal Variables
    private Vector3 m_MoveVector;
    private float m_MoveSpeed;
    private bool b_WasGroundedLastFrame = true;
    private float inputX, inputY;
    private bool drop = false;
    private Vector3 lastPointOnWall;
    private bool bIsTouchingClimbable = false;
    //References
    private CharacterController charController;
    [SerializeField]
    private CameraController cameraController;
    //Set up variables
    private void Start()
    {
        charController = GetComponent<CharacterController>();
        if (charController == null)
            Debug.LogError("Character Controller not set up on Player!");
        if (cameraController == null)
            Debug.LogError("Camera Controller reference not set up in PlayerController!");
    }

    //Main Tick loop
    private void FixedUpdate()
    {
        Input();

        if (!bIsClimbing)
        {
            if (drop)
                drop = false;
            AttachToClimbableWall();
        }

        m_MoveSpeed = bIsWalking ? m_WalkSpeed : m_JogSpeed;

        if (!charController.isGrounded && b_WasGroundedLastFrame)
            m_MoveVector.y = 0f;

        //Check if player is climbing and move according to climb scheme (in y-x plane);
        if (bIsClimbing)
        {
            ClimbMove();
            if (charController.isGrounded && inputY < 0)
            {
                Drop();
                m_MoveVector += transform.forward * -2; 
            }
            if (drop)
            {
                Drop();
            }
            if (IsStillOnClimbableWall())
            {
                if (IsNearLedge())
                {
                    ClimbLedge(lastPointOnWall);
                }
            }
            else
            {
                Drop();
            }
        }
        //Check if player is in contact with ground, and move appropriately (in x-z plane)
        else if (charController.isGrounded)
        {
            GroundMove();
        }
        //If Falling, allow some control
        else
        {
            AirMove();
        }

        charController.Move(m_MoveVector * Time.fixedDeltaTime);
        b_WasGroundedLastFrame = charController.isGrounded;
    }

    //Climbing Functions
    private void AttachToClimbableWall()
    {
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2))
        {
            if (hit.collider.CompareTag("Climbable"))
            {
                if (bIsTouchingClimbable && charController.isGrounded)
                {
                    StartCoroutine(TweenToPosition(transform, hit.point + hit.normal.normalized * (charController.radius + 0.2f) + transform.up * 0.7f, 0.7f, stickToClimbableSpeed));
                    bIsClimbing = true;
                    bIsTouchingClimbable = false;
                }
            }
        }
    }
    private bool IsStillOnClimbableWall()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2))
        {
            return hit.collider.CompareTag("Climbable");
        }
        else
        {
            return false;
        }
    }
    private bool IsNearLedge()
    {
        RaycastHit hit;
        //All but the player layer
        int playerBitmask = ~(1 << LayerMask.NameToLayer("Player"));

        Ray ray = new Ray(transform.position, (transform.forward + transform.up).normalized);
        if (Physics.Raycast(ray, out hit, 3, playerBitmask))
        {
            if (!hit.collider.CompareTag("Climbable"))
            {
                return true;
            }
            else
            {
                lastPointOnWall = hit.point;
                return false;
            }
        }
        else
        {
            return true;
        }
    }
    private void ClimbLedge(Vector3 ledgePoint)
    {
        Vector3 wantedPos1 = transform.position + new Vector3(0, ledgePoint.y - transform.position.y, 0);
        Vector3 wantedPos2 = wantedPos1 + transform.up * 0.5f + transform.forward * 1.5f;
        Queue<TweenToWaypoint> waypointQueue = new Queue<TweenToWaypoint>();
        waypointQueue.Enqueue(new TweenToWaypoint(wantedPos1, 0.8f));
        waypointQueue.Enqueue(new TweenToWaypoint(wantedPos2, 0.4f));
        StartCoroutine(TweenPosQueue(transform, waypointQueue));
    }
    private void Drop()
    {
        bIsClimbing = false;
        drop = false;
    }

    
    //Read input from Input class
    private void Input()
    {
        if (UnityEngine.Input.GetButtonDown("Walk"))
            bIsWalking = true;
        if (UnityEngine.Input.GetButtonUp("Walk"))
            bIsWalking = false;
        inputX = UnityEngine.Input.GetAxis("Horizontal");
        inputY = UnityEngine.Input.GetAxis("Vertical");
        if(UnityEngine.Input.GetButtonDown("Jump"))
            drop = true;
    }

    //Movement Schemes, called every Tick
    private void AirMove()
    {
        Vector3 desiredVel = transform.forward * UnityEngine.Input.GetAxis("Vertical") + transform.right * UnityEngine.Input.GetAxis("Horizontal");
        Vector3 desiredDir = desiredVel.normalized;
        Vector3 previousMove = new Vector3(m_MoveVector.x, 0, m_MoveVector.z);

        //Add movement vector depending on how much air control we have
        Vector3 addMove;
        if (previousMove.magnitude > m_MinAirSpeed)
            addMove = desiredDir * previousMove.magnitude * m_AirControl;
        else
            addMove = desiredDir * m_MinAirSpeed * m_AirControl;

        //The movement is the previous tick movement plus the added vector. Check nextMove doesnt surpass MoveSpeed
        Vector3 nextMove = previousMove + addMove;
        if (nextMove.magnitude > m_MoveSpeed)
            nextMove = nextMove.normalized * m_MoveSpeed;


        //Add Gravity
        nextMove += Physics.gravity * Time.fixedDeltaTime * m_GravityMult;

        m_MoveVector = nextMove;
    }
    private void GroundMove()
    {
        Vector3 desiredVel = transform.forward * UnityEngine.Input.GetAxis("Vertical") + transform.right * UnityEngine.Input.GetAxis("Horizontal");
        Vector3 desiredDir = desiredVel.normalized;
        Vector3 previousMove = new Vector3(m_MoveVector.x, 0, m_MoveVector.z);

        


        //The Movement that will be added to player (calculated from desired movement and previous movement)
        Vector3 addMove;
        if (m_GroundControl * previousMove.magnitude > 1)
            addMove = desiredDir * previousMove.magnitude * m_GroundControl;
        else
            addMove = desiredDir * m_MoveSpeed * m_GroundControl;

        //Apply Friction
        float previousSpeed = previousMove.magnitude;
        if(previousSpeed != 0)
        {
            float drop = previousSpeed * m_Friction * Time.fixedDeltaTime;
            float newSpeed = previousSpeed - drop;
            if (newSpeed < 0)
                newSpeed = 0;
            else
            {
                newSpeed /= previousSpeed;
                previousMove *= newSpeed;
            }
        }

        Vector3 nextMove = previousMove + addMove;
        if (nextMove.magnitude > m_MoveSpeed)
            nextMove = nextMove.normalized * m_MoveSpeed;

        nextMove.y = -m_StickToGround;

        m_MoveVector = nextMove;
    }
    private void ClimbMove()
    {
        
        Vector3 desiredVel = transform.up * UnityEngine.Input.GetAxis("Vertical") + transform.right * UnityEngine.Input.GetAxis("Horizontal");
        Vector3 desiredDir = desiredVel.normalized;

        Vector3 nextMove = desiredDir * m_ClimbSpeed;

        m_MoveVector = nextMove;

    }


    //This makes sure you are not stuck to walls when you come at them at an angle
    void OnControllerColliderHit(ControllerColliderHit hit)
    {

        Vector3 surfaceNormal = hit.normal;
        float projY = Vector3.Project(surfaceNormal, Vector3.up).y;
        if (Vector3.Dot(surfaceNormal, m_MoveVector) < 0 && (projY <= Mathf.Sin(surfAngleThreshold * Mathf.Deg2Rad)))
            m_MoveVector = Vector3.ProjectOnPlane(m_MoveVector, surfaceNormal);

        if (m_MoveVector.magnitude > (m_MoveSpeed))
            m_MoveVector = m_MoveVector.normalized * m_MoveSpeed;
        if (hit.collider.CompareTag("Climbable") && bIsClimbing == false)
        {
            if (Vector3.Dot(m_MoveVector.normalized, hit.normal.normalized) < 0)
                bIsTouchingClimbable = true;
        }
    }


    private struct TweenToWaypoint
    {
        public TweenToWaypoint(Vector3 dest, float dur)
        {
            destination = dest;
            duration = dur;
        }
        public Vector3 destination;
        public float duration;
    }

    //Quality of Life coroutine: Moves through positions in a queue, as provided.
    IEnumerator TweenPosQueue(Transform go, Queue<TweenToWaypoint> waypoints)
    {
        while(waypoints.Count != 0)
        {
            TweenToWaypoint waypoint = waypoints.Dequeue();
            yield return StartCoroutine(TweenToPosition(go, waypoint.destination, waypoint.duration, AnimationCurve.EaseInOut(0,0,1,1)));
        }

    }

    //Move transform to position for given time.
    IEnumerator TweenToPosition(Transform go, Vector3 destination, float duration, AnimationCurve animCurve)
    {
        //yield return new WaitForSeconds(0.1f);
        float journey = 0f;
        while (journey <= duration)
        {
            journey = journey + Time.deltaTime;
            float percent = Mathf.Clamp01(journey / duration);
            float curvePercent = animCurve.Evaluate(percent);
            go.position = Vector3.Lerp(go.position, destination, curvePercent);

            yield return null;
        }
    }

}
