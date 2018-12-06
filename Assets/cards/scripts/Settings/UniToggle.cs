using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UniToggle : MonoBehaviour {

	public MonoBehaviour targetScript;			//input: Reference the script, which should be enabled/disabled.
	public bool enabledOnFirstStartup = false;	//input: On first start-up, should the script be enabled or disabled?
	public string prefsKey = "onButtonScript";	//input: Key for the save-name for this instance.

	//some optional linkings to this script
	public Toggle optToggleShowState;			//Optional input: set a toggle, the value toggle.isOn will be modified by this script
	public Text optTextForState;				//Optional input: set a text, it will be changed every enable/disable of the referenced script
	public string onText = "ON";				//Optional input: text for the optTextForState if ON.
	public string offText = "OFF";				//Optional input: text for the optTextForState if OFF.
	//privates
	private bool __scriptIsEnabled = false;

	void Start () {

		//On first start-up, the key does not exist.
		//Set the key to the designers choice.
		if (PlayerPrefs.HasKey (prefsKey) == false) {
			__scriptIsEnabled = enabledOnFirstStartup;
			saveScriptState ();
		} else {
			loadScriptState();
		}

		testScriptStateOnStart ();

		//set the script-Enable/disable
		enableDisableScript ();
	}

	/*
	 * Function setScriptState
	 * Call this function by a button, external script or even from a toggle to set the 
	 * state of the script. The state will be automatically saved and 
	 * loaded on next start of this script.
	 * 
	 * */
	public void setScriptState(bool enabled){
		//Debug.Log ("set state to " + enabled.ToString ());
		__scriptIsEnabled = enabled;
		saveScriptState ();
		enableDisableScript ();
	}

	/*
	 * Function invertScriptState
	 * Call this function mainly by a button to invert the 
	 * state of the script (ON-> OFF or OFF-> ON). 
	 * Therefore you can switch the script on and off with a single button.
	 * 
	 * The state will be automatically saved and 
	 * loaded on next start of this script.
	 * 
	 * */
	public void btnOnly_invertScriptState(){
		__scriptIsEnabled = !__scriptIsEnabled;
		saveScriptState ();
		enableDisableScript ();
	}


	/*
	 * Save the actual state (enabled or disabled) for the target script.
	 * */
	void saveScriptState(){
		if (__scriptIsEnabled == true) {
			PlayerPrefs.SetInt (prefsKey, 1);
		} else {
			PlayerPrefs.SetInt(prefsKey,0);
		}
	}

	/*
	 * Load the actual state (enabled or disabled) for the target script.
	 * */
	void loadScriptState(){
		int state = PlayerPrefs.GetInt (prefsKey);
		if (state == 0) {
			__scriptIsEnabled = false;
		} else {
			__scriptIsEnabled = true;
		}
	}

	/*
	 * Set the real state of the target script.
	 * */
	void enableDisableScript(){
		if (targetScript != null) {
			targetScript.enabled = __scriptIsEnabled;
		} else {
			Debug.LogError("Script for Enable/Disable not settable on " + name + " object.");
		}

		//if there is a toggle linked to this, show the state on the toggle.
		if (optToggleShowState != null) {
			optToggleShowState.isOn = __scriptIsEnabled;
		}

		//if there is a text gadged linked to this, modify the text string.
		if(optTextForState !=null){
			if(__scriptIsEnabled == true){
				optTextForState.text = onText;
			}else{
				optTextForState.text = offText;
			}
		}
	}

	/*
	 * Test, if the target script is linked and if the script is disabled on boot-up.
	 * */
	void testScriptStateOnStart(){
		if (targetScript != null) {
			if(targetScript.enabled == true){
				Debug.LogError("Linked Monobehaviour is enabled at startup. This can cause Runtime and start problems.");
			}else{
				//Script is available and not activated at start-up. This is ok.
			}
		}else {
			Debug.LogError("Script for Enable/Disable not settable on " + name + " object.");
		}
	}
}
