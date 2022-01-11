using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimReceiver : MonoBehaviour {

	private GunAndMeleeSystem fightingSystem;
	//private Journal journal;
	// Use this for initialization
	void Start () 
	{
		fightingSystem = transform.root.GetComponent<GunAndMeleeSystem>();
		//journal = transform.root.GetComponent<Journal>();
	}
	
	// Update is called once per frame
	//void Update () {
		
	//}
	
    public void SetInComboWindow(int state)
    {
        //if(state == 1)
        //    fightingSystem.SetInComboWindow(true);
        //else
        //    fightingSystem.SetInComboWindow(false);
    }


    public void AllowShooting()
	{
		//fightingSystem.allowShot = true;
	}
	public void ReloadTimer()
	{
		fightingSystem.pistolSoundSource.PlayOneShot(fightingSystem.reloadSound);
		fightingSystem.Reload(false);
		
	}
    public void DealDamageSlash()
    {
        fightingSystem.DoSlashDamage();
    }
    public void EndSlash()
    {
        //fightingSystem.EndSlash();
    }

	//public void StartFlames()
	//{
	//	if(fightingSystem.isSlashing)
	//		fightingSystem.flame.Play();
	//}
	
	//public void SpinSound()
	//{
	//	fightingSystem.pistolSoundSource.PlayOneShot(fightingSystem.pistolSpinSound);
	//}
	
	//public void FinishedIdle()
	//{
	//	fightingSystem.Idle_InspectPistol(false);
	//}
	
	//public void HideJournal()
	//{
	//	journal.journal.SetActive(false);
	//	// Debug.Log("HiddenJournal");
	//	journal.journalMoving = false;
	//}
	
	//public void JournalStoppedMoving()
	//{
	//	journal.journalMoving = false;
	//}
}
