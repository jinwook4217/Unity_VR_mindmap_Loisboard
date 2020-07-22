using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReStart : MonoBehaviour {

	// Use this for initialization

	void Awake()
	{
			
	}
	void Start () {
		GameObject.FindObjectOfType<NodeManager>().ReStart();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
