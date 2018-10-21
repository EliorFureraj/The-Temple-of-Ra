using UnityEngine;
using System.Collections;

public class PriestMummy : Mummy 
{

	
	[SerializeField]
	// private GameObject target;
	public float attackDist = 50;
	private Transform tr;
	private bool inRange;
	public bool isAttacking = false;
	public GameObject fireBall;
	public float damageMin;
	public float damageMax;
	private PlayerStats playerStats;
	public Collider colliders;
	[HideInInspector] public IEnemyState currentState;
    [HideInInspector] public FollowState followState;
    [HideInInspector] public AttackState attackState;
    [HideInInspector] public ReviveState reviveState;
	public GameObject key;
	

	
	void Awake()
	{
		followState = new FollowState(this);
		attackState = new AttackState(this);
		reviveState = new ReviveState(this);
		
	}
	
	void Start()
	{
		healthBarScaled = healthBar.transform.Find("Scaled");
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
		if(!target)
		{
			target = GameObject.FindGameObjectWithTag("Player");
		}
		
		attackDist *= attackDist;
		targetMotor = target.GetComponent<CharacterController>();
		targetTransform = target.transform;
		tr = transform;
		currentState = followState;
			if(!animator)
		animator = GetComponent<Animator>();
		target = GameObject.FindGameObjectWithTag("Player");
		playerStats = target.GetComponent<PlayerStats>();
	}
	
	
	
	void Update()
	{
		if(!isdead && startFight)
		currentState.OnStateUpdate();
	}
	/// 1 - Follow   2 - Attack   3 - Revive


	///////////////////////////////////////////////////////
	// FOLLOW //
	///////////////////////////////////////////////////////
	
	
	///////////////////////////////////////////////////////
	//ATTACK
	///////////////////////////////////////////////////////
	

	
	void ChargeFlame()
	{
		
	}

	
	
	void ChargeEndFlame()
	{
		
	}
	
	
	
	public float force = 10;
	public Transform handTransform;
	
	
	
	public void ReleaseFlame()
	{
		GameObject fireBallRigid;
        fireBallRigid = Instantiate(fireBall, handTransform.position, transform.rotation) as GameObject;
		fireBallRigid.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward * force);
		isAttacking = false;
	}
	
	
	
	
	private Vector3 distanceTemp;
	
	
	
	public bool InRange()
	{
		distanceTemp = targetTransform.position-tr.position;
		if(distanceTemp.sqrMagnitude < attackDist)
		{
			inRange = true;
			return true;
		}
		else
		{
			inRange = false;
			return false;
		}
	}
	
	
	
	private RaycastHit hitLOS;
	
	public bool InLOS()
	{
        
		if(Physics.Raycast(tr.position, Vector3.forward, out hitLOS, attackDist))
		{
			if(hitLOS.collider.CompareTag("Player"))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}

	private float damage;
	
	public void ApplyDamage ()
	{
			damage = Random.Range (damageMin, damageMax);
			playerStats.AddHealth (-damage, true);
	}

	
	
	public void FollowEnemy()
	{
		navAgent.destination = targetTransform.position;
	}
	
	
	
	
	
	public float timeModifier = 1;
	Vector3 attackFuturePos;
	float t = 0;
	
	
	
	public void AttackHelper()
	{
		t = (targetTransform.position - tr.position).magnitude;
		attackFuturePos = (targetTransform.position + targetMotor.velocity * (t/8)) -tr.position;
		
		Quaternion attackRot = Quaternion.LookRotation(attackFuturePos);
		attackRot.x = 0;
		attackRot.z = 0;
		tr.rotation = Quaternion.Slerp(tr.rotation, attackRot, Time.deltaTime * timeModifier);
		
	}
	
	
	
	private float timeStamp;
	public float attackCooldown = 4;
	
	
	
	public void Attack()
	{
		if(timeStamp <= Time.time && isAttacking == false)
		{
			animator.SetTrigger("Attack");
			isAttacking = true;
			timeStamp = Time.time + attackCooldown;
		}
	}

    protected override void Die()
	{
		if(!isdead)
		{
			colliders.enabled = false;
		}
        base.Die();
	}
	
	public override void DropLoot()
	{
		Instantiate (key, transform.position, transform.rotation);
	}
}
