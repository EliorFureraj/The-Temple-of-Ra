using UnityEngine;
using System.Collections;
using System;

public class WalkerMummy : Mummy 
{
    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;

    private float attackDist = 8;
	private Transform tr;
	private bool inRange;
	public bool isAttacking = false;
	private bool inMeleeZone;
	private PlayerStats playerStats;
	public float damageMin;
	public float damageMax;
	public AudioSource audioSource;
	public AudioClip[] thump;

    private bool isMoving = false;
    public bool shouldMove = false;
    private bool allowRotation = true;


public Collider[] colliders;
	

	void Start()
	{
		healthBar = transform.Find("HealthBar");
		healthBarScaled = healthBar.transform.Find("Scaled");
        //navAgent.updatePosition = false;
        base.Start();

	}
	
	void Reset()
	{
		if(!navAgent)
		navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		
		if(!target)
		target = GameObject.FindGameObjectWithTag("Player");
	
		if(!animator)
		animator = GetComponent<Animator>();
	}
	public override void StartManual()
	{		
		base.StartManual();
		
		audioSource = GetComponent<AudioSource>();
		target = GameObject.FindGameObjectWithTag("Player");
		targetMotor = target.GetComponent<CharacterController>();
		targetTransform = target.transform;
		tr = transform;

		if(!animator)
			animator = GetComponent<Animator>();

		playerStats = target.GetComponent<PlayerStats>();
        //animator.SetFloat("Speed", 1);
        
        shouldMove = false;

    }
	
	void Update()
	{
        UpdateAnimation();
		if(!isdead && startFight)
		    FollowEnemy();
	}

    private void UpdateAnimation()
    {
        // Update animation parameters
        animator.SetBool("move", isMoving);
        //GetComponent<LookAt>().lookAtTargetPosition = navAgent.steeringTarget + transform.forward;
    }

    //private Vector3 distanceTemp;
	
	public bool InRange()
	{
		return false;
	}
    int thumpIndex = 1;
	public void Thump()
	{
        AudioSource.PlayClipAtPoint(thump[thumpIndex - 1], transform.position);
      thumpIndex++;
        if (thumpIndex > 3)
            thumpIndex = 1;
	}
    float speedTemp = 0;

    private bool IsFacingEnemy()
    {
        float dotProduct = Vector3.Dot(transform.forward, (targetTransform.position - transform.position).normalized);
        return (Mathf.Abs(1 - dotProduct) < rotationTolerance);
       
    }
    public float rotationTolerance = 0.05f;

    public void FollowEnemy()
	{
        

        if (inMeleeZone && !isMoving)
		{
            if (IsFacingEnemy())
                Attack();
        }
        navAgent.isStopped = !shouldMove;
        if (Mathf.Approximately(navAgent.velocity.sqrMagnitude, 0))
            isMoving = false;
        else
            isMoving = true;

        if (shouldMove)
            navAgent.destination = targetTransform.position;
        if(!isMoving && allowRotation)
            RotateTowards(targetTransform);

    }


    private void RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
    }
    public void StopAttack()
    {
        //if(!inMeleeZone)
        allowRotation = true;
        shouldMove = true;
        //if (!inMeleeZone)
            animator.SetBool("Attack", false);
    }
	private float timeStamp;
	public float attackCooldown = 15;

    int AttackType = 0;
	public void Attack()
	{
		if(timeStamp <= Time.time)
		{
            shouldMove = false;
            allowRotation = false;
            animator.SetInteger("AttackType", AttackType);
            AttackType++;
            if (AttackType > 2)
                AttackType = 0;
            animator.SetBool("Attack",true);
			timeStamp = Time.time + attackCooldown;
		}
	}
	
	
	void OnTriggerEnter(Collider col)
	{
		if(col.CompareTag("Player"))
		{
			inMeleeZone = true;

		}
	}
	
	void OnTriggerExit(Collider col)
	{
		if(col.CompareTag("Player"))
		{
			inMeleeZone = false;
		}
	}


	private float damage;
	public void ApplyDamage (float mult)
	{
		if (inMeleeZone) 
		{
			damage = UnityEngine.Random.Range (damageMin, damageMax);
			playerStats.AddHealth (-damage * mult, true);
		}
	}
	
	protected override void Die()
	{
		if(!isdead)
		{
            allowRotation = false;
            shouldMove = false;
			foreach(Collider i in colliders)
				i.enabled = false;
		}
        base.Die();
    }
}
