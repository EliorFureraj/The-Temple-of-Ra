

public var strafeTiltAmount : float = 10;
public var strafeTiltSpeed : float;
private var strafeTiltPivot : Vector3;
private var initialCameraPosition : Vector3;
private var initialCameraRotation : Quaternion;
private var axis : int;
public var isActive : boolean = true;

function Start()
{
	strafeTiltPivot = transform.position + new Vector3(0,0.2,0);
	initialCameraPosition = transform.position;
	initialCameraRotation = transform.rotation;
}
function Update () 
{
	if(isActive)
    {
		if(Input.GetAxis("Horizontal") > 0)
		{
			TiltOnStrafe(-1);
		}
		else if(Input.GetAxis("Horizontal") < 0)
		{
			TiltOnStrafe(1);
		}
		else if(Input.GetAxis("Horizontal") == 0)
		{
			TiltOnStrafe(0);
		}
	}
	else
		TiltOnStrafe(0);
}

 
 function TiltOnStrafe(axis : int)
 {
	var wantedRotation = initialCameraRotation * Quaternion.AngleAxis(axis * strafeTiltAmount, Vector3.forward);
	transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(Vector3(0, 0, wantedRotation.eulerAngles.z)), Time.deltaTime * strafeTiltSpeed);
 }
