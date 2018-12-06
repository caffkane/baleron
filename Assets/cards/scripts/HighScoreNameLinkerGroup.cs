using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreNameLinkerGroup : MonoBehaviour {

	public static HighScoreNameLinkerGroup instance;

	void Awake(){
		instance = this;
	}

	public HighScoreNameLinker[] hsnl;
	// Use this for initialization
	void Start () {
		//hsnl = GetComponentsInChildren<HighScoreNameLinker> ();
	}

	public void generateLinks(){
		foreach (HighScoreNameLinker a in hsnl) {
            //Debug.LogWarning("hsnl "+a.gameObject.name);
			a.generateHighScoreNameLink ();
		}
	}

	// Update is called once per frame
	void Update () {
	}
}
