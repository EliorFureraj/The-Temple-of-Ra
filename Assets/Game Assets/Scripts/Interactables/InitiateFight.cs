using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InitiateFight : MonoBehaviour 
{
    [SerializeField]
	private Mummy[] mummies;
    [SerializeField]
    private AudioSource playerMusic;
    [SerializeField]
    private AudioClip fireSong;

    private int counterDead = 0;
	private bool enteredOnce = false;
  
    private List<Mummy> mummiesToKill;

    private void Start()
    {
        mummiesToKill = new List<Mummy>();
        foreach(Mummy mummy in mummies)
        {
            mummiesToKill.Add(mummy);
        }
    }

    bool done = false;
    void Update()
    {
        if (enteredOnce && !done)
        {

            foreach (Mummy mummy in mummies)
            {
                if(mummy.isdead)
                {
                    if (mummiesToKill.Contains(mummy))
                        mummiesToKill.Remove(mummy);
                }
            }
            if (mummiesToKill.Count == 0)
            {
                playerMusic.Stop();
                playerMusic.clip = null;
                done = true;
            }
        }
    }
    void OnTriggerEnter(Collider other)
	{
        if (!other.CompareTag("Player"))
            return;
        if (enteredOnce)
            return;
        Initiate();
    }
	
    public void Initiate()
    {
        foreach (Mummy mummy in mummies)
        {
            if (mummy.startFight == false)
            {
                mummy.StartManual();
                mummy.StartFight();
            }
        }
        enteredOnce = true;
        if (playerMusic)
        {
            playerMusic.clip = fireSong;
            playerMusic.Play();
        }
    }
	
}
