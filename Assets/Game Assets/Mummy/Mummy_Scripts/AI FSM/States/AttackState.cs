using UnityEngine;
using System.Collections;

public class AttackState : IEnemyState 
{

	private readonly PriestMummy enemy;
	
	
	
	public AttackState (PriestMummy mummy)
	{
		enemy = mummy;
	}

	public void OnStateEnter()
	{
	}
	
	public void OnStateUpdate()
	{
		if(enemy.InRange())
		{
			enemy.AttackHelper();
			enemy.Attack();
		}
		else
		{
			ToFollowState();
		}
	}
	
	public void OnStateExit()
	{
	}
	
	public void ToFollowState()
	{
		enemy.currentState.OnStateExit();
		enemy.currentState = enemy.followState;
		enemy.currentState.OnStateEnter();
	}
	
	public void ToAttackState()
	{
		enemy.currentState.OnStateExit();
		enemy.currentState = enemy.attackState;
		enemy.currentState.OnStateEnter();
	}
	
	public void ToReviveState()
	{
		enemy.currentState.OnStateExit();
		enemy.currentState = enemy.reviveState;
		enemy.currentState.OnStateEnter();
	}
	
}
