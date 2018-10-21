using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {


    private bool bIsWalking = false;
    private bool bIsClimbing = false;

    [SerializeField]
    private float m_JogSpeed = 3;
    [SerializeField]
    private float m_WalkSpeed = 1.5f;
    [SerializeField]
    private float m_ClimbSpeed = 1f;

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

    [SerializeField]
    private AnimationCurve animationCurve;
    private Vector3 moveVector;
    private float m_MoveSpeed;
    private bool b_WasGroundedLastFrame;
    private float inputX, inputY;
    private bool climbInput;


    private CharacterController charController;
    private void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        Input();
        if (climbInput)
        {
            RaycastHit hit;
            if (!bIsClimbing)
            {

                if (Physics.Raycast(transform.position, transform.forward, out hit, 2))
                {
                    if (hit.collider.CompareTag("Climbable"))
                    {
                            StartCoroutine(TweenToPosition(transform, hit.point + hit.normal.normalized * (charController.radius + 0.2f), 0.7f));
                        bIsClimbing = true;
                    }
                }
            }
            else
            {
                if (Physics.Raycast(transform.position, transform.forward, out hit, 2))
                {
                    if (!hit.collider.CompareTag("Climbable"))
                    {
                        bIsClimbing = false;
                        climbInput = false;
                    }
                }
                else
                {
                    bIsClimbing = false;
                    climbInput = false;
                }

                
                if (Physics.Raycast(Camera.main.transform.position, (transform.forward + transform.up).normalized, out hit, 2))
                {
                    Debug.DrawLine(Camera.main.transform.position, (transform.forward + transform.up).normalized, Color.red);
                    if (hit.collider.CompareTag("Ledge"))
                    {
                        Debug.Log("Hit a ledge");
                        StartCoroutine(TweenToPosition(transform, hit.collider.transform.position + new Vector3(0, charController.height, 0), 1));
                        StartCoroutine(TweenToPosition(transform, hit.collider.transform.position + new Vector3(0, charController.height, charController.radius * 2), 1));
                    }
                }

            }
        }
        m_MoveSpeed = bIsWalking ? m_WalkSpeed : m_JogSpeed;

        if (!charController.isGrounded && b_WasGroundedLastFrame)
            moveVector.y = 0f;

        if (bIsClimbing)
        {
            ClimbMove();
            bIsClimbing = climbInput;
        }
        else if (charController.isGrounded)
        {
            GroundMove();
        }
        else
        {
            AirMove();
        }
        charController.Move(moveVector * Time.fixedDeltaTime);
        b_WasGroundedLastFrame = charController.isGrounded;
    }

    private void Input()
    {
        if (UnityEngine.Input.GetButtonDown("Walk"))
            bIsWalking = true;
        if (UnityEngine.Input.GetButtonUp("Walk"))
            bIsWalking = true;
        inputX = UnityEngine.Input.GetAxis("Horizontal");
        inputY = UnityEngine.Input.GetAxis("Vertical");
        if(UnityEngine.Input.GetButtonDown("Jump"))
            climbInput = !climbInput;
    }

    private void AirMove()
    {
        Vector3 desiredVel = transform.forward * UnityEngine.Input.GetAxis("Vertical") + transform.right * UnityEngine.Input.GetAxis("Horizontal");
        Vector3 desiredDir = desiredVel.normalized;
        Vector3 previousMove = new Vector3(moveVector.x, 0, moveVector.z);

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

        moveVector = nextMove;
    }
    private void GroundMove()
    {
        Vector3 desiredVel = transform.forward * UnityEngine.Input.GetAxis("Vertical") + transform.right * UnityEngine.Input.GetAxis("Horizontal");
        Vector3 desiredDir = desiredVel.normalized;
        Vector3 previousMove = new Vector3(moveVector.x, 0, moveVector.z);

        


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

        moveVector = nextMove;
    }

    private void ClimbMove()
    {
        
        Vector3 desiredVel = transform.up * UnityEngine.Input.GetAxis("Vertical") + transform.right * UnityEngine.Input.GetAxis("Horizontal");
        Vector3 desiredDir = desiredVel.normalized;

        Vector3 nextMove = desiredDir * m_ClimbSpeed;

        moveVector = nextMove;

    }

    public void ClimbLedge(Vector3 posToTween)
    {
        if (bIsClimbing)
        {
            StartCoroutine(TweenToPosition(transform, posToTween, 1));
            climbInput = false;
            bIsClimbing = false;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Vector3 surfaceNormal = hit.normal;
        float projY = Vector3.Project(surfaceNormal, Vector3.up).y;
        if (Vector3.Dot(surfaceNormal, moveVector) < 0 && (projY <= Mathf.Sin(surfAngleThreshold * Mathf.Deg2Rad)))
            moveVector = Vector3.ProjectOnPlane(moveVector, surfaceNormal);
    }


    IEnumerator TweenToPosition(Transform go, Vector3 destination, float duration)
    {
        yield return new WaitForSeconds(0.1f);
        float journey = 0f;
        while (journey <= duration)
        {
            journey = journey + Time.deltaTime;
            float percent = Mathf.Clamp01(journey / duration);
            float curvePercent = animationCurve.Evaluate(percent);
            go.position = Vector3.Lerp(go.position, destination, curvePercent);

            yield return null;
        }
    }

}
