using UnityEngine;
using System.Collections;

public class FollowState : IEnemyState 
{
	private readonly PriestMummy enemy;
	
	public FollowState (PriestMummy mummy)
	{
		enemy = mummy;
	}
	
	public void OnStateEnter()
	{
		enemy.navAgent.Resume();
	}
	
	public void OnStateUpdate()
	{
		if(!enemy.InRange())
		{
			enemy.FollowEnemy();
		}
		else
		{
			ToAttackState();
		}
	}
	
	public void OnStateExit()
	{
		enemy.navAgent.Stop();
	}

	public void ToFollowState()
	{
		Debug.Log("Cant Transition to Self");
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
