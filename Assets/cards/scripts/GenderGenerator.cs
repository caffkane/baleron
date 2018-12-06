
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * The script 'GenderGenerator' shows a pictogram and a text, depending on a
 * value from a value script. It can also be used to show a value dependant pictogram and
 * is not limited to genders. The value is casted to an index and selects the property 
 * to show from the defined list.
 */

public class GenderGenerator : TranslatableContent {

	public static GenderGenerator instance;

	[Tooltip("Define the value type which holds the gender value. This value type should also be linked to the 'Country Name Generator'.")]
	public valueDefinitions.values valueType;
	ValueScript vs;

	void Awake(){
		instance = this;
	}

	void Start(){
		clearUI ();
		StartCoroutine (frameDelay());
		TranslationManager.instance.registerTranslateableContentScript (this);
	}

	//because of the dependency to the value script, which loads at Start(), we need one frame delay to actualize the UI.
	IEnumerator frameDelay(){
		yield return null;
		vs = valueManager.instance.getFirstFittingValue (valueType);
		actualizeUI ();
	}


	[Tooltip("Define the names and pictograms for the available genders.")]
	public gendStringList[] genders;

	[Tooltip("Define the text field for showing the actual gender name.")]
	public Text outText;
	[Tooltip("Define the image for showing the actual gender pictogram.")]
	public Image outImg;


	[System.Serializable]
	public class gendStringList{
		public string gender;
		public Sprite picto;
	}


	Color originalSpriteColor;
	bool colorCopied = false;
	void clearUI(){
		if (outText != null) {
			outText.text = "";
		}
		if (outImg != null) {
			if (colorCopied == false) {
				originalSpriteColor = outImg.color;
				colorCopied = true;
			}
			outImg.color = Color.clear;
		}
	}

	public void actualizeUI(){
		if (vs != null) {
			if (outText != null) {
				outText.text = getGenderText ();
			}
			if (outImg != null) {
				if (colorCopied == true) {
					outImg.color = originalSpriteColor;
				}
				outImg.sprite = getGenderSprite ();
			}
		}
	}


	public string getGenderText()
	{
		if (vs != null) {
			int index = Mathf.RoundToInt (vs.value);
			if (index >= genders.Length) {
				index = genders.Length - 1;
			}
			return TranslationManager.translateIfAvail (genders [index].gender);
		}
		return null;
	}

	public Sprite getGenderSprite()
	{
		if (vs != null) {
			int index = Mathf.RoundToInt (vs.value);
			if (index >= genders.Length) {
				index = genders.Length - 1;
			}
			return genders [index].picto;
		}
		return null;
	}

	/*
	 * Return all possible translatable terms
	 */
	public override List<string> getTranslatableTerms ()
	{
		List<string> terms = new List<string> ();
		terms.Clear ();
		EventScript es;

		foreach (gendStringList gsl in genders) {
			terms.Add (gsl.gender);
		}

		return terms;
	}


}
