using UnityEngine;
using System.Collections;

public class ExitScript : MonoBehaviour {



	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			quit();
		}
	}

	public void quit(){
		Application.Quit ();
	}

}