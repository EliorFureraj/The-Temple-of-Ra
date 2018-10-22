using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public enum CameraControl
    {
        PlayerControlled,
        ScriptControlled,
        Disabled
    }

    [SerializeField]
    private float sensitivity;
    [SerializeField]
    private float minX, maxX;

    public CameraControl cameraControlState = CameraControl.Disabled;

    //References
    [SerializeField]
    private Transform playerTransform;
    //internal variables
    private float xRotation, yRotation;
    private Transform lookAtTarget;
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
        switch(cameraControlState)
        {
            case CameraControl.PlayerControlled:
                GetInput();
                SetRotation();
                break;
            case CameraControl.ScriptControlled:

                break;
            case CameraControl.Disabled:
                break;
        }
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

    public void StopLookAt()
    {
        StopAllCoroutines();
        cameraControlState = CameraControl.PlayerControlled;
    }

    public void LookAtTransform(Vector3 tr, float time = 1)
    {
        cameraControlState = CameraControl.ScriptControlled;
        StartCoroutine(LookAt(tr, time, AnimationCurve.EaseInOut(0, 0, 1, 1)));
    }


    public void SetRotation(float x, float y)
    {
        xRotation = x;
        yRotation = y;
    }

    private IEnumerator LookAt(Vector3 tr, float time, AnimationCurve animCurve = null)
    {
        if (animCurve == null)
            animCurve = AnimationCurve.Linear(0, 0, 1, 1);

        float speed = 1 / time;
        float interp = 0;

        float startXRot = transform.localRotation.eulerAngles.x;
        float startYRot = playerTransform.localRotation.eulerAngles.y;

        while (interp <= time)
        {
            yield return null;
            Vector3 lookDir = tr - transform.position;
            Quaternion lookRot = Quaternion.LookRotation(lookDir);

            float endXRot = lookRot.eulerAngles.x;
            float endYRot = lookRot.eulerAngles.y;
            interp += Time.smoothDeltaTime;
            float percent = Mathf.Clamp01(interp / time);
            float percentCurve = animCurve.Evaluate(percent);

            CalculateShortestPathRotation(ref startXRot, ref endXRot, startXRot, endXRot);
            CalculateShortestPathRotation(ref startYRot, ref endYRot, startYRot, endYRot);

            xRotation = Mathf.Lerp(startXRot, endXRot, percentCurve);
            yRotation = Mathf.Lerp(startYRot, endYRot, percentCurve);
            SetRotation();
        }
        cameraControlState = CameraControl.PlayerControlled;
    }

    private void CalculateShortestPathRotation(ref float newStart, ref float newEnd, float start, float end)
    {
        if (end < 0f && start > 0f)
        {
            if (Mathf.Abs(end - start) > Mathf.Abs(start - (end + 360f)))
                newEnd = end + 360f;
        }
        else if (start < 0f && end > 0f)
        {
            if (Mathf.Abs(end - start) > Mathf.Abs((start + 360f) - end))
                newStart = start + 360f;
        }
    }
}
