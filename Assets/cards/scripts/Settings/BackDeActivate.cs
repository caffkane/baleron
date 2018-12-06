using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackDeActivate : MonoBehaviour {

	public GameObject deactivateGO;
	public GameObject activateGO;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			deactivate();
		}
	}

	public void deactivate(){
		deactivateGO.SetActive (false);
		activateGO.SetActive (true);
	}
}
