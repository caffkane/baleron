using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class valueDependentEnable : MonoBehaviour {

	[Tooltip("Reference to an value script, which contains the compare information for enabling/disabling the gameobject.")]
	public ValueScript value;
	[Tooltip("Gameobject, which should be enabled/disabled depending on value. The gameobject is enabled, if the value of the value script is higher than the limit.")]
	public GameObject go;
	[Tooltip("Limit for enable/disable.")]
	public float limitToEnable = 0.5f;

	
	void Update () {
		if (value.value >= limitToEnable && go.activeSelf == false) {
			go.SetActive (true);
		}
		if (value.value < limitToEnable && go.activeSelf == true) {
			go.SetActive (false);
		}
	}

}
