using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class ValueScript : MonoBehaviour {

	[Tooltip("Define your type for the value here. Per value script only one value type is allowed.")]
	public valueDefinitions.values valueType;

	private string identifier = "valuename";
	[ReadOnlyInspector]public float value = 0f;

	public bool debugValueChanges = false;

	void Awake(){
		identifier = valueType.ToString ();
		loadValue();
	}

	void Start(){
		valueManager.instance.registerValueScript (this);

		testActualizeIconsBaseOnValue ();
		actualizeIconsBaseOnValue ();
	}

	public void actualizeUI(float uiValue){
		if (UserInterface.uiScrollbar != null) {
			UserInterface.uiScrollbar.size = uiValue / limits.max;
		}
		if (UserInterface.textValue != null) {
			UserInterface.textValue.text = uiValue.ToString(UserInterface.formatter);
		}
		if (UserInterface.uiSlider != null) {
			UserInterface.uiSlider.value = uiValue / limits.max;
		}
	}


	//replace the icons depending on the value
	[System.Serializable]
	public class valueDependantIcon{
		[Tooltip("Define the image(s), which should be changed depending on the value. If no baseIcon is defined, nothing will be changed.")]
		public Image[] baseIcons;
		[Tooltip("Define at which value the base icons will be replaced.")]
		public valueToIcon[] valueIcon;
	}
	[System.Serializable]
	public class valueToIcon{
		public float minValue = 0f;
		public Sprite icon = null;
	}


	[System.Serializable]
	public class uiConfig{
		[Tooltip("If the value shall be displayed as a filling bar it can be a scrollbar or a slider. Define your preference here.")]
		public Scrollbar uiScrollbar;
		[Tooltip("If the value shall be displayed as a filling bar it can be a scrollbar or a slider. Define your preference here.")]
		public Slider uiSlider;
		[Tooltip("The speed of filling the bar, if the value changes.")]
		[Range(0.1f,100f)]public float lerpSpeed = 10f;
		[Tooltip("Define the format for displaying the value. \nE.g. 0 or # for only whole numbers.\nE.g. #.00 to show two following digits.")]
		public string formatter = "0.##";
		[Tooltip("The actual lerped/filling value.")]
		[ReadOnlyInspector]public float lerpedValue = 0f;
		[Tooltip("If the value is displayed as text, define the text field here.")]
		public Text textValue;

		[Tooltip("The value manager can show a change of the value to the user depending on 'showActualization'")]
		public bool showActualization = true;
		[Tooltip("The value manager can show a change of the value with this miniature sprite.")]
		public Sprite miniatureSprite;
		[Tooltip("The icon of the value type depending on the content of the value type.")]
		public valueDependantIcon valueDependingIcons;
		public uiValueChange valueChangePreview;
	}
	[System.Serializable]
	public class uiValueChange
	{
		[Tooltip("An upcomiming value change can be indicated by an image. Define the image where to show it.\nThe sprites are defined at the script 'ValueChangePreview'.")]
		public Image valueChangeImage;
		[Tooltip("An upcomiming value change can be shown in a text field. Define where to show it.")]
		public Text valueChangeText;
	}
	[Tooltip("For each value script there can be different ways to display the value. Define the details here.")]
	public uiConfig UserInterface;

	//For the value change preview, how will it change based on the actual movement of the card.
	//Depending on the combination of possible value changes an image for the preview is selected.
	public class c_nextValueChage{
		public bool valueWasSetForChange = false;	//the value will can change (but it can be unknown how much)
		public bool randomIndependant = true;		//the change of the value is unknown
		public float valueChange = 0f;				//the value will change by this value
	}
	public c_nextValueChage nextValueChange = new c_nextValueChage();
	public void setPreviewValues(float valueChange, bool randomIndependant){

		if (nextValueChange.valueWasSetForChange == true) {
			//The value was marked for a change more than once. Therefore the change which will occure is unknown.
			nextValueChange.randomIndependant = false;
		} else {
			//This is the first time for this cycle that a value change is possible. Take the randomness information from calling script
			nextValueChange.randomIndependant = randomIndependant;
		}

		nextValueChange.valueChange = valueChange;

		nextValueChange.valueWasSetForChange = true;
	}
	public void clearPreviewValues(){
		nextValueChange.valueWasSetForChange = false;
		nextValueChange.randomIndependant = true;
		nextValueChange.valueChange = 0f;

		if (UserInterface.valueChangePreview.valueChangeImage != null) {
			UserInterface.valueChangePreview.valueChangeImage.sprite = ValueChangePreview.instance.noChanges;
		}
		if (UserInterface.valueChangePreview.valueChangeText != null) {
			UserInterface.valueChangePreview.valueChangeText.text = "";
		}
	}

	void Update(){
		//UserInterface.lerpedValue = Mathf.Lerp (UserInterface.lerpedValue, value, UserInterface.lerpSpeed * Time.deltaTime);

		UserInterface.lerpedValue = MathExtension.linearInterpolate (UserInterface.lerpedValue, value, UserInterface.lerpSpeed * Time.deltaTime);

		actualizeUI (UserInterface.lerpedValue);
	
		actualizePreviewElements ();

		actualizeIconsBaseOnValue ();
	}
		
	void actualizePreviewElements(){
		if (UserInterface.valueChangePreview.valueChangeImage != null) {	
			UserInterface.valueChangePreview.valueChangeImage.sprite = ValueChangePreview.instance.getPreviewSprite (nextValueChange.valueWasSetForChange,nextValueChange.valueChange, nextValueChange.randomIndependant);
			float scale = ValueChangePreview.instance.getPreviewSize (nextValueChange.valueWasSetForChange, nextValueChange.valueChange, nextValueChange.randomIndependant);
			UserInterface.valueChangePreview.valueChangeImage.rectTransform.localScale = new Vector3 (scale, scale, 1f);
		}
		if (UserInterface.valueChangePreview.valueChangeText != null) {
			UserInterface.valueChangePreview.valueChangeText.text = ValueChangePreview.instance.getPreviewText (nextValueChange.valueWasSetForChange,nextValueChange.valueChange, nextValueChange.randomIndependant);;
		}
	}

	private float oldValueForIcon = 0f;
	void actualizeIconsBaseOnValue(){

		//only compute, if the value changed
		if (value != oldValueForIcon) {

			//only replace icons, if one ore more target images are available
			if (UserInterface.valueDependingIcons.baseIcons != null && UserInterface.valueDependingIcons.valueIcon != null) {
				if (UserInterface.valueDependingIcons.baseIcons.Length > 0 && UserInterface.valueDependingIcons.valueIcon.Length > 0) {

					//get through the list to get the new icon
					Sprite changedSprite = null;
					for(int i = 0; i<UserInterface.valueDependingIcons.valueIcon.Length; i++){
						if (value >= UserInterface.valueDependingIcons.valueIcon [i].minValue) {
							changedSprite = UserInterface.valueDependingIcons.valueIcon [i].icon;
						}
					}

					if (changedSprite != null) {
						foreach (Image im in UserInterface.valueDependingIcons.baseIcons) {
							if (im != null) {
								im.sprite = changedSprite;
							} else {
								Debug.LogWarning ("One or more target images (icons) are not defined for the value dependent replacement at the valueScript for '" + valueType.ToString () + "'.");
							}
						}
					}
				}
			}
		}
		oldValueForIcon = value;
	}
	//selftest for correct configuration
	void testActualizeIconsBaseOnValue(){
		//test for unfitting combinations
		if (UserInterface.valueDependingIcons.baseIcons.Length >0 && UserInterface.valueDependingIcons.valueIcon.Length == 0) {
			Debug.LogWarning ("ValueScript: depending icons are not fitting. Number of target icons are "+UserInterface.valueDependingIcons.baseIcons.Length.ToString()+" but source icons are "+UserInterface.valueDependingIcons.valueIcon.Length.ToString()+" at the valueScript for '" + valueType.ToString ()+"'.");
		}
		if (UserInterface.valueDependingIcons.baseIcons.Length == 0 && UserInterface.valueDependingIcons.valueIcon.Length > 0) {
			Debug.LogWarning ("ValueScript: depending icons are not fitting. Number of target icons are "+UserInterface.valueDependingIcons.baseIcons.Length.ToString()+" but source icons are "+UserInterface.valueDependingIcons.valueIcon.Length.ToString()+" at the valueScript for '" + valueType.ToString ()+"'.");
		}

		//test for selection order
		float limit = -1f;
		bool orderError = false;
		if (UserInterface.valueDependingIcons.valueIcon.Length > 0) {

			limit = UserInterface.valueDependingIcons.valueIcon [0].minValue;
			if (limit > 0.01f) {
				orderError = true;
			}
			limit = -1f;
			for(int i = 0; i<UserInterface.valueDependingIcons.valueIcon.Length; i++){
				if (limit >= UserInterface.valueDependingIcons.valueIcon [i].minValue) {
					orderError = true;
				}
				limit = UserInterface.valueDependingIcons.valueIcon [i].minValue;
			}

			if (orderError == true) {

				string wrongOrder = "";
				for (int i = 0; i < UserInterface.valueDependingIcons.valueIcon.Length; i++) {
					wrongOrder += UserInterface.valueDependingIcons.valueIcon [i].minValue + " - ";
				}

				Debug.LogWarning ("ValueScript: value depending icon order not fitting. " +
					"First minValue should start with 0 and then ascend for each element. " +
					"e.g. 0 - 10 - 30 -. " +
					"Order at the valueScript for '" + valueType.ToString ()+"' is: " + 
				wrongOrder + ".");
				
			}
		}
	}

	[System.Serializable]
	public class valueLimits
	{
		[Tooltip("What is the minimum possible value?")]
		public float min = 0f;
		[Tooltip("What is the maximum possible value?")]
		public float max = 100f;
		[Tooltip("For initialization with random values: What is the minimum random value.")]
		public float randomMin = 0f;
		[Tooltip("For initialization with random values: What is the maximum random value.")]
		public float randomMax = 100f;
	}

	public valueLimits limits;

	public void limitValue(){
		if (value < limits.min) {
			value = limits.min;
			events.OnMin.Invoke();
		}
		if (value > limits.max) {
			value = limits.max;
			events.OnMax.Invoke();
		}
	}

	/*
	 * Increas or decrease a value. 
	 */
	public float addValue (float increment){

		if (debugValueChanges == true) {
			Debug.Log ("Value '" + valueType.ToString () + "': " + value.ToString () + " increment by " + increment.ToString ());
		}

		value += increment;
		limitValue ();
		if (increment >= 0f) {
			events.OnIncrease.Invoke ();
		} else {
			events.OnDecrease.Invoke ();
		}
		saveValue ();

		if (debugValueChanges == true) {
			Debug.Log ("Value '" + valueType.ToString () + "' is now " + value.ToString () + " (after limiter)");
		}

		return value;
	}

	/*
	 * Set a value to an defined value.
	 */
	public float setValue(float newValue){
		value = newValue;
		limitValue ();
		saveValue ();
		return value;
	}

	[Tooltip("'keepValue' blocks the randomization of the value on a new game start. On the first startup of the game, the value is randomized between 'Limits.RandomMin' and 'Limits.RandomMax' (Accessable from Inspector).")]
	public bool keepValue = false;

	/*
	 * Randomize a value within the defined range. Helpfull for initialization of a new game.
	 */

	public float setValueRandom(){
			value = Random.Range (limits.randomMin, limits.randomMax);
			limitValue ();
			saveValue ();
		return value;
	}

	public void newGameStart(){
		if (keepValue == false) {
			setValueRandom ();
		}
	}


	public float multiplyValue (float multiplier){
		value *= multiplier;
		limitValue ();
		if (multiplier >= 1f) {
			events.OnIncrease.Invoke();
		}else {
			events.OnDecrease.Invoke ();
		}
		saveValue ();
		return value;
	}

	void loadValue(){
		if (SecurePlayerPrefs.HasKey (identifier)) {
			value = SecurePlayerPrefs.GetFloat (identifier);
		} else {
			setValueRandom ();
		}
	}

	void saveValue(){
		SecurePlayerPrefs.SetFloat (identifier, value);
	}

	public void saveMinMax(){
		float min = SecurePlayerPrefs.GetFloat (identifier+"_min");
		float max = SecurePlayerPrefs.GetFloat (identifier+"_max");

		if(SecurePlayerPrefs.HasKey(identifier+"_min")){
			if (value < min) {
				SecurePlayerPrefs.SetFloat (identifier+"_min",value);
			}	
		}else{
			SecurePlayerPrefs.SetFloat (identifier+"_min",value);
		}

		if (value > max) {
			SecurePlayerPrefs.SetFloat (identifier + "_max", value);
		}

	}

	public float getMaxValue(){
		return SecurePlayerPrefs.GetFloat (identifier+"_max");
	}

	public float getMinValue(){
		return SecurePlayerPrefs.GetFloat (identifier+"_min");
	}

	void OnDestroy(){
		saveValue ();
	}

	[System.Serializable] public class mEvent : UnityEvent {}

	[System.Serializable]
	public class valueEvents{
		public mEvent OnIncrease;
		public mEvent OnDecrease;
		public mEvent OnMax;
		public mEvent OnMin;
	}

	[Tooltip("Events are triggered on value changes or if the value is at one of its limits.")]
	public valueEvents events;


}
