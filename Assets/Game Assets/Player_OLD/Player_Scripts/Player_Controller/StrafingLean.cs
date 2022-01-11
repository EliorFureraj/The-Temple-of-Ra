using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafingLean : MonoBehaviour
{
    [SerializeField]
    private float strafeTiltAmount = 10;
    [SerializeField]
    private float strafeTiltSpeed;

    private bool isActive = true;

    private Vector3 strafeTiltPivot;
    private Vector3 initialCameraPosition;
    private Quaternion initialCameraRotation;
    private int axis;
    

    // Start is called before the first frame update
    void Start()
    {
        strafeTiltPivot = transform.position + new Vector3(0, 0.2f, 0);
        initialCameraPosition = transform.position;
        initialCameraRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                TiltOnStrafe(-1);
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                TiltOnStrafe(1);
            }
            else if (Input.GetAxis("Horizontal") == 0)
            {
                TiltOnStrafe(0);
            }
        }
        else
            TiltOnStrafe(0);
    }

    public void SetActive(bool state)
    {
        isActive = state;
    }

    public bool GetActive()
    {
        return isActive;
    }


    private void TiltOnStrafe(int axis)
    {
        var wantedRotation = initialCameraRotation * Quaternion.AngleAxis(axis * strafeTiltAmount, Vector3.forward);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(new Vector3(0, 0, wantedRotation.eulerAngles.z)), Time.deltaTime * strafeTiltSpeed);
    }


}
