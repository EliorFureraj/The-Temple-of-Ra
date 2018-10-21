using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Damageable))]
public abstract class Mummy : MonoBehaviour {

	public bool isdead = false;
	public GameObject target;
	protected CharacterController targetMotor;
	protected Transform targetTransform;
	public float health = 100f;
	public float healthMax = 100f;
	public Transform healthBar;
	public Transform healthBarScaled;
	public GameObject blood;
	public Animator animator;
	public UnityEngine.AI.NavMeshAgent navAgent;
	public GameObject[] lootPrefabs;	
	public bool startFight = false;
	

    //Delegates 

	public virtual void Start()
	{
		healthBar.gameObject.SetActive(false);
        GetComponent<Damageable>().onDamaged += Damage;
	}
	
	public virtual void StartManual()
	{
		healthBar.gameObject.SetActive(true);
		healthBar.localScale = new Vector3(healthMax/100,1,1);
	}

    bool hitReaction = false;
    protected virtual void Damage(float amount, string cause)
    {
        AddHealth(-amount);
        if (cause == "pistol")
        {
            Debug.Log("Reached Animation Handler, Pistol: " + cause);
            if (hitReaction)
                animator.SetTrigger("HitRight");
            else
                animator.SetTrigger("HitLeft");
            hitReaction = !hitReaction;
        }
        else if(cause.Contains("slash"))
        {
            Debug.Log("Reached Animation Handler, Slash: " + cause);
            if (cause.Contains("0"))
                animator.SetTrigger("HitLeft");
            if (cause.Contains("1") || cause.Contains("2"))
                animator.SetTrigger("HitRight");
        }
    }
	public virtual void AddHealth (float dmg)
	{
		health += dmg;
		if(health != 0)
			healthBarScaled.localScale = new Vector3(health/100,1,1);
		if (health > healthMax)
			health = healthMax;
		if (health <= 0)
		{
			health = 0;
			healthBar.gameObject.SetActive(false);
            Die();
		}
		
		if(startFight == false)
		{
			StartFight();
			StartManual();
		}
		
	}

    protected virtual void Die()
    {
        if (!isdead)
        {
            isdead = true;
            animator.SetTrigger("Die");
            navAgent.isStopped = true;
            navAgent.enabled = false;
            DropLoot();
        }
    }

    public virtual void OnParticleCollision (GameObject other)
	{
		
		if (other.CompareTag("ParticleFireShoot")) 
		{
			AddHealth(-0.3f);
		}
	}
	
	public virtual void StartFight()
	{
		if(!startFight)
		{
			animator.SetBool("StartFight", true);
			startFight = true;
		}
	}
	
	protected ParticleSystem bloodClone;

	
	public virtual void DropLoot()
	{
		int randValue = UnityEngine.Random.Range (0, lootPrefabs.Length);
        Instantiate(lootPrefabs[randValue], transform.position, Quaternion.identity);
    }
}
