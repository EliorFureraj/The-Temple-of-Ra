using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeMarker : MonoBehaviour {

    [SerializeField]
    private Transform ledgePosition;
    [SerializeField]
    private Transform ledgeLook;

    public Transform GetLedgePosition()
    {
        return ledgePosition;
    }
    public Transform GetLedgeLook()
    {
        return ledgeLook;
    }

    public Vector3 GetForwardDirection()
    {
        return transform.forward;
    }

}
