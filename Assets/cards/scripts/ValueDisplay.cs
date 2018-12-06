using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ValueDisplay : MonoBehaviour {

	[Tooltip("Reference to an value script, which contains the compare information for enabling/disabling the gameobject.")]
	//value script is automatically linked later
	ValueScript vs;

	[Tooltip("Define which kind of value you want to display.")]
	public valueDefinitions.values valueTyp;

	[Tooltip("Define the text display for the current value.")]
	public Text currentValueText;
	[Tooltip("Define the text display for the minimal value of the script within this game.")]
	public Text minValueText;
	[Tooltip("Define the text display for the maximal value of the script within this game.")]
	public Text maxValueText;


	[Tooltip("Define the format for displaying the value. \nE.g. 0 or # for only whole numbers.\nE.g. #.00 to show two following digits.")]
	public string formatter = "0";
	[Tooltip("Define a multiplier for displaying of the value. This does't change the original value.")]
	public float displayMultiplier = 1f;

	public void showMinMaxValue(){

		vs = valueManager.instance.getFirstFittingValue (valueTyp);

		if (vs != null) {
			if (maxValueText != null) {
				maxValueText.text = (vs.getMaxValue ()*displayMultiplier).ToString (formatter);
			}
			if (minValueText != null) {
				minValueText.text = (vs.getMinValue ()*displayMultiplier).ToString (formatter);
			}
			if (currentValueText != null) {
				currentValueText.text = (vs.value*displayMultiplier).ToString (formatter);
			}
		}
	}

	void Start(){
		showMinMaxValue ();
	}
}
