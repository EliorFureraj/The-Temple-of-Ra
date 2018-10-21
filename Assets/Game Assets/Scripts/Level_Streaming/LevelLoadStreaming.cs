using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoadStreaming : MonoBehaviour {

	private int currentSceneIndex;
	private int activeSceneIndex;
	
	public bool exit = false;
	
	public GameManager gameManager;
	// public GameObject light;
	// private GameObject canvas;
	public GameObject loadBar;
	
	private int lastLoaded;

	// Use this for initialization
	void Start () 
	{
		// PlayerDependencyManager pdm = GetComponent<PlayerDependencyManager>();
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		// canvas = pdm.canvas;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	private GameObject light;
	// SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1, LoadSceneMode.Additive);
	// gameObject.scene.buildIndex
	void OnTriggerEnter(Collider other)
	{
        if (!other.CompareTag("Player"))
            return;
		currentSceneIndex = gameObject.scene.buildIndex;
		activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
		
		if(currentSceneIndex != 0f)
		{

			if(gameManager.exits[currentSceneIndex] == false)
			{
				if(currentSceneIndex-1 == activeSceneIndex)
				{
					SceneManager.MoveGameObjectToScene(gameManager.player, SceneManager.GetSceneByBuildIndex(currentSceneIndex));
					SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(currentSceneIndex));
					SceneManager.UnloadSceneAsync(currentSceneIndex-1);
					gameManager.inSceneLimbo = false;
				}
				else
				{
					if(gameManager.inSceneLimbo)
					{
						// light = GameObject.Find("Sun"+currentSceneIndex);
						SceneManager.UnloadSceneAsync(lastLoaded);
						light.SetActive(true);
						gameManager.inSceneLimbo = false;
					}
					else
					{
						light = GameObject.Find("Sun"+currentSceneIndex);
						gameManager.inSceneLimbo = true;
						StartCoroutine(AsynchronousLoad(currentSceneIndex + 1));
						lastLoaded = currentSceneIndex + 1;
						Debug.Log("Loaded Scene: " + (currentSceneIndex + 1));
					}
				}
				gameManager.exits[currentSceneIndex] = true;
			}
			else
			{
				if(currentSceneIndex+1 == activeSceneIndex )
				{
					SceneManager.MoveGameObjectToScene(gameManager.player, SceneManager.GetSceneByBuildIndex(currentSceneIndex));
					SceneManager.SetActiveScene(SceneManager.GetSceneAt(currentSceneIndex));
					SceneManager.UnloadSceneAsync(currentSceneIndex+1);
					gameManager.inSceneLimbo = false;
				}
				else
				{
					if(gameManager.inSceneLimbo)
					{
						// light = GameObject.Find("Sun"+currentSceneIndex);
						SceneManager.UnloadSceneAsync(lastLoaded);
						light.SetActive(true);
						gameManager.inSceneLimbo = false;
					}
					else
					{
						light = GameObject.Find("Sun"+currentSceneIndex);
						gameManager.inSceneLimbo = true;
						StartCoroutine(AsynchronousLoad(currentSceneIndex - 1));
						lastLoaded = currentSceneIndex - 1;
						Debug.Log("Loaded Scene: " + (currentSceneIndex - 1));
					}
				}
				gameManager.exits[currentSceneIndex] = false;
			}
		}
		
	}
	
	IEnumerator AsynchronousLoad (int scene)
	{
		yield return null;
		loadBar = gameManager.loadBar;
		AsyncOperation ao = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
		ao.allowSceneActivation = false;
		loadBar.SetActive(true);
		while (! ao.isDone)
		{
			float progress = Mathf.Clamp01(ao.progress / 0.9f);
			
			loadBar.transform.localScale =  new Vector3((progress), 1, 1);
			// Debug.log("Loading progress: " + (progress * 100) + "%");
	 
			if (ao.progress == 0.9f)
			{
				
				// SceneManager.MoveGameObjectToScene(gameManagerO, SceneManager.GetSceneByBuildIndex(scene));
				// SceneManager.MoveGameObjectToScene(gameManager.player, SceneManager.GetSceneByBuildIndex(scene));
				// SceneManager.MoveGameObjectToScene(gameManager.canvas, SceneManager.GetSceneByBuildIndex(scene));
				// SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetSceneByBuildIndex(scene));
				// Debug.Log("Press a key to start");
				// if (Input.anyKey)
					ao.allowSceneActivation = true;
			}
			
			yield return null;
		}
		loadBar.SetActive(false);
		light.SetActive(false);
	}
}
