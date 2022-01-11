﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class GameManager : MonoBehaviour {

	// public static GameManager manager; 
	public GameObject player;
	public GameObject playerPrefab;
	public GameObject flyPrefab;
	public GameObject playerSpawn;
    public CameraController camController;
    public CanvasSingleton canvas;
	
	[System.NonSerialized]
	public bool[] exits;
	// [System.NonSerialized]
	public GameObject loadBar;
	private bool isPlayer = true;
	
	private GameObject playerObj;
	private GameObject flyObj;
	private int currentscene = 0;
	
	public float currentFOV = 80;
	
	public float normalFOV = 80;
	
	public float zoomFOV = 50;
	
	// public bool cursorShouldBeLocked = true;
	public Camera handCamera;
	[System.NonSerialized]
	public bool inSceneLimbo = false;
	// Use this for initialization
	public bool lockCursor = true;
	private bool m_cursorIsLocked = true;

	
	public void Awake()
    {
		DontDestroyOnLoad(this);
			 
		if (FindObjectsOfType(GetType()).Length > 1)
		{
            Destroy(gameObject);
        }
		Scene scene = SceneManager.GetActiveScene();
		currentscene = scene.buildIndex;
		
		exits = new bool[4];
		exits[0] = false;
		exits[1] = false;
		exits[2] = false;
		exits[3] = false;
		if(!playerSpawn)
			playerSpawn = GameObject.Find("PlayerSpawn");
		if(GameObject.Find("Player") == null)
		{
			playerObj = Instantiate(playerPrefab, playerSpawn.transform.position, playerSpawn.transform.rotation);
            playerObj.name = "Player";
		}
        player = playerObj;
		handCamera = player.transform.Find("Body/Hands/CameraHands").GetComponent<Camera>();
        camController = player.transform.Find("Body/Camera").GetComponent<CameraController>();


		
		if(GameObject.Find("Canvas") != null)
		{
			canvas = GameObject.Find("Canvas").GetComponent<CanvasSingleton>();
			loadBar = canvas.loadBar;
		}
		else
		{	
			GameObject canvasObj = Instantiate(Resources.Load("Canvas"), Vector3.zero, Quaternion.identity) as GameObject;
			canvasObj.name = Resources.Load("Canvas").name;
			canvas = canvasObj.GetComponent<CanvasSingleton>();
			loadBar = canvas.loadBar;
		}

        DisablePlayer();
        SetCursorLock(true);
	}
	
	IEnumerator Delay()
	{
		yield return new WaitForSeconds(1);
		AudioListener.volume = 1f;
	}
	
	
	
    public void SetCursorLock(bool value)
    {
		lockCursor = value;
		if(!lockCursor)
		{//we force unlock the cursor if the user disable the cursor locking helper
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		else
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}
	

	public void UpdateCursorLock()
	{
		if (lockCursor)
			InternalLockUpdate();
	}

	private void InternalLockUpdate()
	{

		if (m_cursorIsLocked)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else if (!m_cursorIsLocked)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}	
	
	void FixedUpdate()
	{
		UpdateCursorLock();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Camera.main.fieldOfView = Mathf.Clamp(currentFOV, 50, 80);
		//handCamera.fieldOfView = Mathf.Clamp(currentFOV, 20, 60);
	}
	
	public void EnablePlayer()
	{
        player.GetComponent<PlayerController>().receivesInput = true;
        player.GetComponent<GunAndMeleeSystem>().enabled = true;
        player.transform.Find("Body").GetComponent<StrafingLean>().SetActive(true);
        camController.cameraControlState = CameraController.CameraControl.PlayerControlled;

        StartCoroutine("Delay");
	}
	
	public void DisablePlayer()
	{
        player.GetComponent<PlayerController>().receivesInput = false;
        player.GetComponent<GunAndMeleeSystem>().enabled = false;
		player.transform.Find("Body").GetComponent<StrafingLean>().SetActive(false);
        camController.cameraControlState = CameraController.CameraControl.Disabled;
        AudioListener.volume = 0f;
	}
	
	public void StopTime()
	{
		Time.timeScale = 0;
	}
	
	public void ResumeTime()
	{
		Time.timeScale = 1;
	}
}