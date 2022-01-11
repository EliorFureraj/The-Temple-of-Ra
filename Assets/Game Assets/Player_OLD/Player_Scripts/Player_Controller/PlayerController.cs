using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {


    private bool bIsWalking = false;
    private bool bIsClimbing = false;

    public bool receivesInput = true;
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
    private float m_terminalVelocity = 56;
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

    int playerBitmask;

    //Internal Variables
    private Vector3 m_MoveVector;
    private float m_MoveSpeed;
    private bool b_WasGroundedLastFrame = true;
    private float inputX, inputY;
    private bool drop = false;
    private Vector3 lastPointOnWall;
    private bool bIsTouchingClimbable = false;
    private LedgeMarker currentLedgeMarker = null;
    private bool bBeingPutInDescentPos = false;

    private bool overridenAddVelocity = false;
    private Vector3 addMoveVector;
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

        playerBitmask = ~(1 << LayerMask.NameToLayer("Player"));
    }

    //Main Tick loop
    private void FixedUpdate()
    {
        if(receivesInput)
            GetInput();
        m_MoveSpeed = bIsWalking ? m_WalkSpeed : m_JogSpeed;

        if (!charController.isGrounded && b_WasGroundedLastFrame)
            m_MoveVector.y = 0f;

        //Check if player is climbing and move according to climb scheme (in y-x plane);
        if (bIsClimbing)
        {
            if (cameraController.cameraControlState != CameraController.CameraControl.BodyLocked && cameraController.cameraControlState != CameraController.CameraControl.ScriptControlled)
                cameraController.LockBody();
            Climb();
        }
        else
        {

            if (IsStandingOnDescentMarker())
            {
                //Debug.Log("Standing on Descent Marker");
                if (Input.GetButtonDown("Activate"))
                {
                    StartCoroutine(MovePlayerToDescentPosition());
                }
            }

            //Check if player is in contact with ground, and move appropriately (in x-z plane)
            if (charController.isGrounded)
            {
                SearchForClimbableSurface();
                GroundMove();
            }
            //If Falling, allow some control
            else
            {
                AirMove();
            }
        }

        if(overridenAddVelocity)
        {
            m_MoveVector += addMoveVector;
            overridenAddVelocity = false;
        }
        charController.Move(m_MoveVector * Time.fixedDeltaTime);
        Debug.Log("Player Vel:\n" + Mathf.RoundToInt(m_MoveVector.x) + "," + Mathf.RoundToInt(m_MoveVector.y) + "," + Mathf.RoundToInt(m_MoveVector.z));
        b_WasGroundedLastFrame = charController.isGrounded;
    }

    private void Climb()
    {
        ClimbMove();
        if (charController.isGrounded && inputY < 0)
        {
            Drop();
            AddVelocity(transform.forward * -2);
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
            if(!bBeingPutInDescentPos && cameraController.cameraControlState != CameraController.CameraControl.ScriptControlled)
                Drop();
        }
    }
    //Climbing Functions
    private void SearchForClimbableSurface()
    {
        RaycastHit hit;
        if (bIsTouchingClimbable && Physics.Raycast(transform.position, transform.forward, out hit, 3))
        {
            if (hit.collider.CompareTag("Climbable"))
            {             
                bIsClimbing = true;
                bIsTouchingClimbable = false;
            }
        }
    }
    private bool IsStillOnClimbableWall()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(transform.position, transform.forward * 2,Color.blue);
        if (Physics.Raycast(ray, out hit, 3, playerBitmask, QueryTriggerInteraction.Ignore))
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


        Ray ray = new Ray(transform.position, (transform.forward + transform.up / 5).normalized);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
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
        Vector3 addVec = transform.up * 2 + transform.forward * 1;
        AddVelocity(addVec);
        Drop();

        ////Using Old method of Tweening player to proper position is very choppy
        //Vector3 wantedPos1 = transform.position + new Vector3(0, ledgePoint.y - transform.position.y, 0);
        //Vector3 wantedPos2 = wantedPos1 + transform.up * 0.5f + transform.forward * 1.5f;
        //Queue<TweenToWaypoint> waypointQueue = new Queue<TweenToWaypoint>();
        //waypointQueue.Enqueue(new TweenToWaypoint(wantedPos1, 0.8f));
        //waypointQueue.Enqueue(new TweenToWaypoint(wantedPos2, 0.4f));
        //StartCoroutine(TweenPosQueue(transform, waypointQueue));
    }
    private void Drop()
    {
        if (!bIsClimbing || bBeingPutInDescentPos)
        {
            drop = false;
            return;
        }
        StartCoroutine(cameraController.FreeBody());
        bIsClimbing = false;
        drop = false;
    }

    private bool IsStandingOnDescentMarker()
    {
        if (!bIsTouchingClimbable)
            return false;

        bool isUnder = false;
        RaycastHit hit;
        
        for(int i = 0; i < 6; i++)
        {
            float value = Mathf.InverseLerp(0, 5, i);
            Ray ray = new Ray(transform.position, transform.up * -1 + transform.forward * Mathf.Cos(value * Mathf.PI * 2) + transform.right * Mathf.Sin(value * Mathf.PI * 2));
            if (Physics.Raycast(ray, out hit, 2, playerBitmask, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.CompareTag("Climbable"))
                {
                    currentLedgeMarker = hit.collider.GetComponent<LedgeMarker>();
                    isUnder = true;
                }
            }
        }
        return isUnder;
    }

    private IEnumerator MovePlayerToDescentPosition()
    {
        bBeingPutInDescentPos = true;
        Debug.Log("In coroutine");
        receivesInput = false;
        Vector3 pos = currentLedgeMarker.GetLedgePosition().position;

        cameraController.LookAtTransform(currentLedgeMarker.GetLedgeLook().position, 0.8f);
        
        Queue<TweenToWaypoint> qt = new Queue<TweenToWaypoint>();
        TweenToWaypoint t1 = new TweenToWaypoint(new Vector3(pos.x, transform.position.y, pos.z), 0.5f);
        TweenToWaypoint t2 = new TweenToWaypoint(pos, 0.5f);
        qt.Enqueue(t1);
        qt.Enqueue(t2);

        bIsClimbing = true;
        yield return StartCoroutine(TweenPosQueue(transform, qt));
        bIsClimbing = true;

        receivesInput = true;
        bBeingPutInDescentPos = false;
        bIsTouchingClimbable = false;
    }

    //Read input from Input class
    private void GetInput()
    { 
        if (Input.GetButtonDown("Walk"))
            bIsWalking = true;
        if (Input.GetButtonUp("Walk"))
            bIsWalking = false;
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");
        if (Input.GetButtonDown("Jump"))
            drop = true;

    }

    //Movement Schemes, called every Tick
    private void AirMove()
    {
        Vector3 desiredVel = transform.forward * UnityEngine.Input.GetAxis("Vertical") + transform.right * UnityEngine.Input.GetAxis("Horizontal");
        Vector3 desiredDir = desiredVel.normalized;
        Vector3 previousMove = new Vector3(m_MoveVector.x, 0, m_MoveVector.z);
        Vector3 previousMoveY = new Vector3(0, m_MoveVector.y, 0);
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
        nextMove += previousMoveY + Physics.gravity * m_GravityMult * Time.fixedDeltaTime;
        nextMove.y = Mathf.Min(nextMove.y, m_terminalVelocity);
        m_MoveVector = nextMove;
    }
    private void GroundMove()
    {
        Vector3 desiredVel = transform.forward * UnityEngine.Input.GetAxis("Vertical") + transform.right * UnityEngine.Input.GetAxis("Horizontal");
        Vector3 desiredDir = Vector3.ClampMagnitude(desiredVel, 1f);
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

        int direction = (cameraController.LookingUpPercentage() < -0.7f ? -1 : 1);
        Vector3 desiredVel = transform.up * direction * UnityEngine.Input.GetAxis("Vertical") + transform.right * UnityEngine.Input.GetAxis("Horizontal");
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

        //Keep the velocity in check
        if (m_MoveVector.sqrMagnitude > (m_MoveSpeed * m_MoveSpeed))
            m_MoveVector = m_MoveVector.normalized * m_MoveSpeed;

        if (hit.collider.CompareTag("Climbable") && bIsClimbing == false)
        {
            bIsTouchingClimbable = true;
        }
    }

    //Use with care, directly modifies the vector used by Move function
    private void AddVelocity(Vector3 addVector)
    {
        overridenAddVelocity = true;
        addMoveVector = addVector;
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
            yield return StartCoroutine(TweenToPosition(go, waypoint.destination, waypoint.duration, AnimationCurve.Linear(0,0,1,1)));
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
            if ((destination - go.position).sqrMagnitude < 0.0025f)
                break;
            yield return null;
        }
    }

}
