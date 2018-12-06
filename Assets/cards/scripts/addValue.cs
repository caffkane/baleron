using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Helper script: modify values by an event.
 */

public class addValue : MonoBehaviour {


	[Tooltip("Define the value changes when calling 'addValues()'")]
	public EventScript.resultModifier[] valuesToChange;


	public void addValues(){
		foreach (EventScript.resultModifier rm in  valuesToChange) {
			valueManager.instance.changeValue (rm.modifier, rm.valueAdd);
		}
	}
}
