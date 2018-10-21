using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour {


    public delegate void Damage(float dmg, string cause);
    public event Damage onDamaged;

	public virtual void CauseDamage(float damage, string cause)
    {
        onDamaged(damage, cause);
    }

}
