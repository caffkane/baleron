using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Helper script: modify values by an event.
 */

public class addValueToValue : MonoBehaviour {


	[Tooltip("Define the value changes when calling 'addValues()'")]
	public resultModifierForAddingValueToValue[] valuesToChange;

	[System.Serializable]
	public class resultModifierForAddingValueToValue{
		public valueDefinitions.values lArgument;
		public float multiplier = 1.0f;
		public valueDefinitions.values rArgument;
	}

	public void addValues(){
		float rValue = 0f;
		foreach (resultModifierForAddingValueToValue vtv in  valuesToChange) {
			rValue =  valueManager.instance.getFirstFittingValue(vtv.rArgument).value;
			valueManager.instance.changeValue (vtv.lArgument, vtv.multiplier * rValue);
		}
	}
}
