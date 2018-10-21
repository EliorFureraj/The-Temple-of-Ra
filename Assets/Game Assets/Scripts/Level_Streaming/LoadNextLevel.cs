using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextLevel : MonoBehaviour {


	public GameManager gameManager;

	public Animator fadePanel;

	private bool entered = false;
	
	private GameObject player = null;
	// Use this for initialization
	void Start () 
	{
		StartCoroutine(LateStart(2));

	}
	

	IEnumerator LateStart(float waitTime)
		 {
			 yield return new WaitForSeconds(waitTime);
			 // Debug.Log("Late Start, Fadepanel");

			 fadePanel.SetTrigger("StartLevel");
            StartAll();

         }

	void StartAll()
	{
		gameManager.EnablePlayer();
	}

	void DisableEverything()
	{
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		player = gameManager.player;
		gameManager.DisablePlayer();
	}
	void ActivateNote()
	{
		// SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}
}
