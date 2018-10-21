using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbLedge : MonoBehaviour {

    [SerializeField]
    private Transform ledgePosition;


    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        if (ledgePosition == null)
    //        {
    //            Debug.Log("Ledge Position not defined");
    //            return;
    //        }
    //        other.GetComponent<PlayerController>().ClimbLedge(ledgePosition.position);
    //    }
    //}
}
