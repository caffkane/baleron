using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowGameLog : MonoBehaviour {

	public Text logText;

	// Use this for initialization
	void Start () {
		logText.text = GameLogger.instance.getGameLog ();
	}

}
