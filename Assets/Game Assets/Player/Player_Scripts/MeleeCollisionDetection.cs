using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeCollisionDetection : MonoBehaviour {

    GunAndMeleeSystem fightSystem;
    List<Damageable> targetList = new List<Damageable>();
	// Use this for initialization
	void Start () {
        fightSystem = transform.root.GetComponent<GunAndMeleeSystem>();
	}

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        Damageable target = other.gameObject.GetComponent<Damageable>();
        if (target != null)
        {
            if(!targetList.Contains(target))
                targetList.Add(target);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Damageable target = other.gameObject.GetComponent<Damageable>();
        if (target != null)
        {
            if (targetList.Contains(target))
                targetList.Remove(target);
        }
    }

    public bool GetTargetsInMelee(ref List<Damageable> targets)
    {
        if (targetList.Count == 0)
            return false;
        else
        {
            targets = targetList;
            return true;
        }
    }
}
