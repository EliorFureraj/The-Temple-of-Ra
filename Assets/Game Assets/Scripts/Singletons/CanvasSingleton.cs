using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSingleton : MonoBehaviour {


	public GameObject loadBar;
	public void Awake()
    {
		DontDestroyOnLoad(this);
			 
		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
		}
	}
	void Start()
	{
		loadBar = transform.Find("LoadBarBack/LoadBar").gameObject;
	}
}
