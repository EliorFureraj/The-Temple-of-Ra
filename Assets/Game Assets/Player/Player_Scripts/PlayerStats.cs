using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerStats : MonoBehaviour 
{

	private float health = 100;
	public int healthMax = 100;

	private float will = 100;
	public float willMax = 100;
	
	// public bool cursorShouldBeLocked =  true;
	
	public bool menuToggle = false;
	
	public bool hasExplosive = false;
	
	public bool hasCrown = false;
	


	/////////////////////////////////////
	//REFERENCES//
	[SerializeField]
	private GameObject cameraFPS;
	
	[SerializeField]
	private Animation cameraAnim;
	
	[SerializeField]
	private Image fadePanel;	
	
	[SerializeField]
	private GameObject WillBar;
	
	[SerializeField]
	private GameObject HealthBar;
	
	[SerializeField]
	private GameObject menuCanvas;
	
	[SerializeField]
	private Graphic[] menuChildren;
	
	// [SerializeField]
	// private CameraShake cameraShake;
	
	
	private GameManager gameManager;
	private Transform canvas;
	
	private FirstPersonController fpController;

	//private Journal journal;
	[SerializeField]
	private Animator animator;

    public bool cheatMode = false;

	
	void Start() 
	{
		PlayerDependencyManager pdm = GetComponent<PlayerDependencyManager>();
		
		if(!cameraFPS)
			cameraFPS = Camera.main.gameObject;
		if(!cameraAnim)
			cameraAnim = cameraFPS.GetComponent<Animation>();
		//if(!journal)
		//	journal = GetComponent<Journal>();
		
		canvas = pdm.canvas;
		if(!HealthBar)
			HealthBar = canvas.Find("HUD/StatsPanel/HealthText/HealthBarBack/HealthBarPlayer").gameObject;
		if(!WillBar)
			WillBar = canvas.Find("HUD/StatsPanel/WillText/WillBarBack/WillBar").gameObject;
		if(!menuCanvas)
			menuCanvas = canvas.Find("MenuCanvas").gameObject;
		
		gameManager = pdm.gameManager;
		
		
		fpController = GetComponent<FirstPersonController>();
		// if(!fadePanel)
			// fadePanel = GameObject.Find("FadePanel").GetComponent<Image>();
		
		
		if(menuChildren.Length == 0)
			menuChildren = menuCanvas.transform.GetChild(0).GetComponentsInChildren<Graphic>();
		
		
	}
	
	void Update () 
	{
		if(Input.GetButtonDown("Menu"))
		{
			OpenMenu(menuToggle);
		}
	}
	
	public bool ReturnExplosive()
	{
		return hasExplosive;
	}
	
	
	
	public float GetHealth()
	{
		return health;
	}

	public void AddHealth(float adder, bool shock)
	{
        if (cheatMode)
            return;
		health += adder;
		if(health > 0)
		{

		}
		else
		{	
			StartCoroutine(Die());
		}
		
		if(health > healthMax)
			health = healthMax;
		if(health < 0)
			health = 0;
		
		HealthBar.transform.localScale =  new Vector3(health / 100, 0.8f, 0.8f);
	}
	
	
	
	public float GetWill()
	{
		return will;
	}

	public void AddWill(float adder)
	{
		// if(infiniteManaPerk == false)
		// {
			will += adder;
			
			
		// }
		
		if(will > willMax)
			will = willMax;
		if(will < 0)
			will = 0;
		WillBar.transform.localScale = new Vector3(will / 100, 0.8f, 0.8f);
	}
	
	
	IEnumerator Die()
	{
		cameraAnim.Play();
		yield return new WaitForSeconds(2);
		menuCanvas.GetComponent<Animator>().SetTrigger("FadeIn");
		yield return new WaitForSeconds(1);
		SceneManager.LoadScene("Die");
	}
	
	
	void OpenMenu(bool v)
	{
		////journal.menuOpen = !v;
		//gameManager.SetCursorLock(v);
		//menuToggle = !menuToggle;
		//if(v == false)
		//{
			
		//	menuCanvas.GetComponent<Animator>().SetTrigger("FadeIn");
		//	//if(journal.isJournalOut == false)
		//	//{
		//	//	gameManager.DisablePlayer();
		//	//	gameManager.StopTime();
		//	//}
		//}
		//else
		//{
		//	// FadeMenu(menuChildren, 0f, 1f);

		//	menuCanvas.GetComponent<Animator>().SetTrigger("FadeOut");
		//	//if(journal.isJournalOut == false)
		//	//{
		//	//	gameManager.EnablePlayer();
		//	//	gameManager.ResumeTime();
		//	//}
		//}
	}
	
	

}
