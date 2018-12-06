using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackActivate : MonoBehaviour {

	public GameObject activateGO;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			activate();
		}
	}

	public void activate(){
		activateGO.SetActive (true);
	}
}
