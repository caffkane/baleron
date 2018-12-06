using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class saveSlider : MonoBehaviour {


	public float valueOnFirstStartup = 1.0f;	//Designer input: On first start-up, what is the Slider value
	public bool useConstantKey = true;			//Designer input: Use the designers key or the unique id of the object 
	public string prefsKey = "slider_0";		//Designer input: Key for the save-name for this instance.
	
	//some optional linkings to this script
	public Slider optSliderOutput;				//Optional designer input: set a slider, it will be set by this script
	public Text optTextSliderValuetext;			//Optional designer input: set a text, it will show the actual value of the slider

	private bool showDebugInformation = false;
	private int callCounter = 0;
	private float m_sliderValue;
	public float savedSliderValue;

	void mDebug(string msg){
		if (showDebugInformation == true) {
			Debug.Log(msg);
		}
	}

	void Start(){
		if (useConstantKey == false) {
			prefsKey = "slider_"+GetInstanceID().ToString();
		}

		loadSliderValue ();
		firstloadUI ();
	}

	void loadSliderValue(){
		if(PlayerPrefs.HasKey(prefsKey)){
			m_sliderValue = PlayerPrefs.GetFloat (prefsKey);
		}else{
			m_sliderValue = valueOnFirstStartup;
			saveSliderValue ();
		}
	}

	void prepareForSaveSliderValue(){
		StopAllCoroutines ();
		StartCoroutine (saveKeyDelayed ());

	}

	//make sure, the key is not saved on every fast change.
	//It is saved, if the value does not change for a certain amount of time.
	//If it changes before, the save is aborted by 'StopAllCoroutines()' and a new timer starts.
	IEnumerator saveKeyDelayed(){
		callCounter++;						//generating debug information

		//don't use
		//		yield return new WaitForSeconds (0.5f);
		//because it will block with timeScale is 0 (e.g. the game is paused)

		//better: look for timeout each frame. After saving the routine exits, 
		//then no performance will be 'consumed'.
		float targetTime = Time.realtimeSinceStartup + 0.3f;
		while (Time.realtimeSinceStartup<targetTime) {
			yield return null;
		}

		saveSliderValue ();
	}

	void saveSliderValue (){
		mDebug("save slider to " + prefsKey+ ". " + callCounter.ToString() + " calls ignored.");
		callCounter = 0;
		PlayerPrefs.SetFloat (prefsKey, m_sliderValue);
		savedSliderValue = m_sliderValue;
	}

	public void setSliderValue(float sliderValue){
		m_sliderValue = sliderValue;
		prepareForSaveSliderValue ();
		actualizeUI ();
	}

	void actualizeUI (){
		if (optTextSliderValuetext) {
			optTextSliderValuetext.text = m_sliderValue.ToString("0.00");
		}
	}

	void firstloadUI(){
		if (optSliderOutput != null) {
			optSliderOutput.value = m_sliderValue;
		}
		actualizeUI ();
	}
}
