using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Damageable))]
public class RadioHit : MonoBehaviour {

    public GameObject radioNew;
    public GameObject radioBroken;
    public ParticleSystem part;
    public InitiateFight initFight;
	// Use this for initialization
	void Start ()
    {
        GetComponent<Damageable>().onDamaged += Break;
    }

    protected void Break(float dmg, string cause)
    {
        part.Play();
        GetComponent<AudioSource>().Stop();
        initFight.Initiate();
        if (cause == "slash")
        {
            radioNew.SetActive(false);
            radioBroken.SetActive(true);
        }
            
    }

	// Update is called once per frame
	void Update () {
		
	}
}
