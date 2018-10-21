using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField]
    private float sensitivity;
    [SerializeField]
    private float minX, maxX;

    //References
    [SerializeField]
    private Transform playerTransform;
    //internal variables
    private float xRotation, yRotation;
	// Use this for initialization
	void Start ()
    {
        if (playerTransform == null)
            playerTransform = transform.root;
        else
        {
            xRotation = transform.eulerAngles.x;
            yRotation = playerTransform.localRotation.eulerAngles.y;
        }
	}
	
    private void GetInput()
    {
        float deltaX = Input.GetAxis("Mouse Y");
        float deltaY = Input.GetAxis("Mouse X");
        if (deltaX != 0 && deltaY != 0)
        {
            xRotation -= deltaX * sensitivity;
            yRotation += deltaY * sensitivity;
        }
    }

    private void SetRotation()
    {
        xRotation = Normalize(xRotation);
        yRotation = Normalize(yRotation);

        xRotation = Mathf.Clamp(xRotation, minX, maxX);

        playerTransform.localRotation = Quaternion.Euler(0, yRotation, 0);
        transform.localRotation = Quaternion.Euler(xRotation, 0, transform.localRotation.eulerAngles.z);

    }
	// Update is called once per frame
	void FixedUpdate ()
    {
        GetInput();
        SetRotation();
	}
    //Keep angles within -180 to 180
    private float Normalize(float f)
    {
        if (f >= 180f)
            return f - 360f;
        else if (f < -180f)
            return f + 360f;
        else
            return f;
    }
}
