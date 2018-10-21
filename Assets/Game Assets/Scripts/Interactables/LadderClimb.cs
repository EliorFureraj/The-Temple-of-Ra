using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class LadderClimb : MonoBehaviour {

    [SerializeField]
    private bool bIsClimbingLadder = false;

    private GameObject Player;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<FirstPersonController>().isActive = false;
            other.GetComponent<FirstPersonController>().bIsGravityOn = false;
            Player = other.gameObject;
            bIsClimbingLadder = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<FirstPersonController>().isActive = true;
            other.GetComponent<FirstPersonController>().bIsGravityOn = true;

            bIsClimbingLadder = false;
        }
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(bIsClimbingLadder)
        {
            float direction = Input.GetAxis("Vertical");
            if(Vector3.Dot(Camera.main.transform.forward.normalized, Vector3.up) < 0.5f)
            {
                direction = -direction;
            }
            
            if (direction > 0)
            {
                Player.GetComponent<CharacterController>().Move(Time.fixedDeltaTime * Vector3.up * 6);
                
            }
            else if (direction < 0)
            {
                
                Player.GetComponent<CharacterController>().Move(Vector3.up * Time.fixedDeltaTime * -2);
            }
            else
            {
                if (!Player.GetComponent<CharacterController>().isGrounded)
                {
                    Player.GetComponent<FirstPersonController>().bIsGravityOn = false;
                }
            }

            if (Input.GetButtonDown("Jump"))
            {
                bIsClimbingLadder = false;
                Player.GetComponent<FirstPersonController>().isActive = true;
                Player.GetComponent<FirstPersonController>().bIsGravityOn = true;
            }
        }
	}
}
