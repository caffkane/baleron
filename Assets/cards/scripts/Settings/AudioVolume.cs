using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AudioVolume : MonoBehaviour {

	public bool soundActivated = true;
	public Toggle Sound_toggle;					//Settings box. either yes/no
	
	private string keySound = "sound_onOff";

	void Awake(){
		AudioListener.volume = 0f;
	}


	// Use this for initialization
	void Start () {
		computeConfig ();

	}

	public void changeVolumeSetting(){
		bool actualSetting = Sound_toggle.isOn;
		
		if (actualSetting) {
			PlayerPrefs.SetInt(keySound,1);
		} else {
			PlayerPrefs.SetInt(keySound,-1);
		}

		computeConfig ();
	}

	void computeConfig(){
		int sound_config = 0;
		sound_config = PlayerPrefs.GetInt (keySound);
		if (sound_config == 0) {
			PlayerPrefs.SetInt(keySound,1);
			sound_config = 1;
			soundActivated = true;
		}
		if (sound_config > 0) {
			soundActivated = true;
			Sound_toggle.isOn = true;
		} else {
			soundActivated = false;
			Sound_toggle.isOn = false;
		}
	
		setVolume (soundActivated);
	}

	void setVolume(bool volume){
		if (volume == false)
						AudioListener.volume = 0f;
				else
						AudioListener.volume = 1f;

		}


}
