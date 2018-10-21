using UnityEngine;
using System.Collections;

public class ReviveState : IEnemyState 
{
	private readonly PriestMummy enemy;
	

	public ReviveState (PriestMummy mummy)
	{
		enemy = mummy;
	}
	
	public void OnStateEnter()
	{
		
	}
	
	public void OnStateUpdate()
	{
		
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
		Debug.Log("Cant Transition to Self");
	}
}
