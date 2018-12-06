using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomElementWithPropability : ScriptableObject {


	[System.Serializable]
	public class propElement
	{
		public GameObject go;
		public float propability;
	}

	void Awake(){
		elements = new List<propElement> ();
	}

	public float overallPropability = 0f;

	public List <propElement> elements;

	public void resetElements(){
		if (elements != null) {
			elements.Clear ();
		} else {
			elements = new List<propElement> ();
			elements.Clear ();
		}
	}

	public void addElement(GameObject go, float propability){
		propElement e = new propElement();
		e.go = go;
		e.propability = propability;
		elements.Add (e);
	}
		
	void calculateOverallPropability(){
		overallPropability = 0f;
		foreach (propElement e in elements) {
			overallPropability += e.propability;
		}
	}

	public GameObject getRandomElement(){
		calculateOverallPropability ();

		float rndResult = Random.Range (0f, overallPropability);

		foreach (propElement e in elements) {
			rndResult -= e.propability;
			if (rndResult > 0f) {
				//no our gameobject
			}else{
				//this is the random gameobject
				return e.go;
				//break;
			}
		}

		//something went wrong. return the first
		return elements[0].go;

	}
		
}
