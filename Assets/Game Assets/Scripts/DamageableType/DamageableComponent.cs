using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableComponent : Damageable {

    
    [SerializeField]
    private bool bIsCriticalSpot = true;

    [SerializeField]
    private float damageMultiplier = 4;

    [SerializeField]
    private Damageable mainDamageReceiver;
    
	// Use this for initialization
	void Start ()
    {
		if(mainDamageReceiver == null)
        {
            mainDamageReceiver = transform.root.GetComponent<Damageable>();
        }
	}

    public override void CauseDamage(float dmg, string cause)
    {
        if (bIsCriticalSpot)
        {
            cause += "+CriticalSpot";
            dmg *= damageMultiplier;
        }
        if (cause.Contains("slash"))
            return;
        mainDamageReceiver.CauseDamage(dmg, cause);
    }
}
