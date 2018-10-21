using UnityEngine;
using System.Collections;

public interface IEnemyState
{

	void OnStateEnter();
	
	void OnStateUpdate();
	
	void OnStateExit();
	
	void ToFollowState();
	
	void ToAttackState();
	
	void ToReviveState();
	
}
