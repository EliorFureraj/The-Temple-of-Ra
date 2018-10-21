using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AmmoPickup : MonoBehaviour
{

    public int bulletNumberMin = 10;
    public int bulletNumberMax = 20;
    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        other.gameObject.GetComponent<GunAndMeleeSystem>().AddAmmo(Random.Range(bulletNumberMin, bulletNumberMax));
        gameObject.SetActive(false);
    }
}
