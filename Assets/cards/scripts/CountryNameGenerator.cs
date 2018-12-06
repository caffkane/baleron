
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Country Name Generator generates a random given name - sur name combination,
 * depending on a random country. This script was mainly written for the game
 * 'Swipe my life' to give the player an identity. The given names are in combination
 * with a gender (only two genders, no apache helicopters :( )
 * To show the gender pictogram the script 'Gender Generator' is used in addition.
 * The randomization of the values are depending on value scripts to save the 
 * combinations.
 */

public class CountryNameGenerator : TranslatableContent {

	public static CountryNameGenerator instance;

	//Definition of the genders. If you want an apache helicopter, add it here.
	[System.Serializable]
	public enum genderTypes{
		male,
		female
	}

	[Tooltip("The actual gender of the player identity.")]
	[ReadOnlyInspector]public genderTypes gender; 

	void Start(){
		clearUI ();
		StartCoroutine (oneFrame ());
		TranslationManager.instance.registerTranslateableContentScript (this);
	}

	void clearUI(){
		if (countryText != null) {
			countryText.text = "";
		}
		if (nameText != null) {
			nameText.text = "";
		}
	}

	//Because the value scripts are registering and loading at Start(), we 
	//need one frame delay to get the registered scrips from the value script manager.
	IEnumerator oneFrame(){
		yield return null;
		createValueScriptLinks ();
		actualizeTexts();
	}

	void createValueScriptLinks(){
		vs_gender = valueManager.instance.getFirstFittingValue (vs_type_gender);
		vs_name = valueManager.instance.getFirstFittingValue (vs_type_givenName);
		vs_surname = valueManager.instance.getFirstFittingValue (vs_type_surname);
		vs_country = valueManager.instance.getFirstFittingValue (vs_type_country);
	}

	void Awake(){
		instance = this;
	}

	[Tooltip("List of possible countries and country dependant names.")]
	public subStringList[] Countries;

	[Tooltip("Text field to display the country.")]
	public Text countryText;
	[Tooltip("Text field to display the name.")]
	public Text nameText;

	[Tooltip("Value type to define the country.")]
	public valueDefinitions.values vs_type_country;
	ValueScript vs_country;
	[Tooltip("Value type to define the given name.")]
	public valueDefinitions.values vs_type_givenName;
	ValueScript vs_name;
	[Tooltip("Value type to define the surname.")]
	public valueDefinitions.values vs_type_surname;
	ValueScript vs_surname;
	[Tooltip("Values type, which holds the gender. This value of the script is defined by 'Country Name Generator', not otherwise.")]
	public valueDefinitions.values vs_type_gender;
	ValueScript vs_gender;

	[System.Serializable]
	public class nameGenderLink{
		public string name;
		public genderTypes gender;
	}

	[System.Serializable]
	public class subStringList{
		[Tooltip("Country of the random player identity.")]
		public string listEntry;
		[Tooltip("List of possible combinations of given name and gender. The lenght of the list should be the same for every country.")]
		public nameGenderLink[] nameComb;
		[Tooltip("List of possible combinations of surnames. The lenght of the list should be the same for every country.")]
		public string[] surname;
	}


	/*
	 * Actualize the text fields with the name and country.
	 */
	public void actualizeTexts(bool force = false){

		if (GameStateManager.instance.gamestate == GameStateManager.Gamestate.gameActive || force == true) {
			if (countryText != null) {
				countryText.text = getCountryTranslatedStringFromValue ();
			}
			if (nameText != null) {
				nameGenderLink ngl = getNameAndGenderFromValue ();
				if (ngl != null) {
					nameText.text = ngl.name + " " + getSurnameFromValue ();
				} else {
					nameText.text = "";
				}
			}

			//expand this, if you want an apache helicopter. To show the apache pictogram modify the 'Gender Generator' script.
			gender = getNameAndGenderFromValue ().gender;
			if (gender != null) {
				if (gender == genderTypes.male) {
					vs_gender.setValue (0f);
				} else if (gender == genderTypes.female) {
					vs_gender.setValue (1f);
				}
			}
		}
	}

	/*
	 * Load the given name and gender from their value script 
	 */
	public nameGenderLink getNameAndGenderFromValue(){
		if (vs_name != null) {
			int index = Mathf.RoundToInt (vs_name.value);
			if (index >= Countries [getCountryIndexFromValue ()].nameComb.Length) {
				index = Countries [getCountryIndexFromValue ()].nameComb.Length - 1;
			}
			nameGenderLink nameGender = Countries [getCountryIndexFromValue ()].nameComb [index];

			return nameGender;
		}
		return null;
	}
	/*
	 * Load the surname from its value script 
	 */
	public string getSurnameFromValue(){
		if (vs_surname != null) {
			int index = Mathf.RoundToInt (vs_surname.value);
			if (index >= Countries [getCountryIndexFromValue ()].surname.Length) {
				index = Countries [getCountryIndexFromValue ()].surname.Length - 1;
			}
			return Countries [getCountryIndexFromValue ()].surname [index];
		}
		return "";
	}
	/*
	 * Load the country index from its value script 
	 */
	public int getCountryIndexFromValue()
	{
		if (vs_country != null) {
			int index = Mathf.RoundToInt (vs_country.value);
			if (index >= Countries.Length) {
				index = Countries.Length - 1;
			}
			return index;
		}
		return -1;
	}

	/*
	 * Get the given name, surname and country as one combined string.
	 */
	public string getCountryNameText(){
		string retText = "";
		nameGenderLink ngl = getNameAndGenderFromValue ();
		if (ngl != null) {
			retText += ngl.name + " " + getSurnameFromValue () + ", " + getCountryTranslatedStringFromValue ();
		} else {
			retText = "";
		}
		return retText;
	}

	/*
	 * Get the country name, translated if available. 
	 */
	public string getCountryTranslatedStringFromValue()
	{
		return TranslationManager.translateIfAvail( Countries[ getCountryIndexFromValue() ].listEntry );
	}

	/*
	 * Return all possible translatable terms
	 */
	public override List<string> getTranslatableTerms ()
	{
		List<string> terms = new List<string> ();
		terms.Clear ();
		EventScript es;

		foreach (subStringList ssl in Countries) {
			terms.Add (ssl.listEntry);
			//don't translate given name and surname?
		}

		return terms;
	}
}



	

