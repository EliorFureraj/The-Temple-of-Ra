using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDependencyManager : MonoBehaviour {
	
	[System.NonSerialized]
	public GameManager gameManager;
	[System.NonSerialized]
	public Transform canvas;
	// Use this for initialization
	void Start () 
	{
		if(GameObject.Find("GameManager") != null)
		{
			gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		}
		else
		{
			GameObject gameManagerobj;
			gameManagerobj = Instantiate(Resources.Load("GameManager"), Vector3.zero, Quaternion.identity) as GameObject;
			gameManagerobj.name = Resources.Load("GameManager").name;
			gameManager = gameManagerobj.gameObject.GetComponent<GameManager>();
			
		}
		if(GameObject.Find("Canvas") != null)
		{
			canvas = GameObject.Find("Canvas").transform;
		}
		else
		{
			GameObject canvasObj;
			canvasObj = Instantiate(Resources.Load("Canvas"), Vector3.zero, Quaternion.identity) as GameObject;
			canvasObj.name = Resources.Load("Canvas").name;
			canvas = canvasObj.transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
