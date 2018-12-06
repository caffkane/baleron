using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class sliderToVolume : MonoBehaviour {

	public Slider volumeSlider;

	public void setVolume(float value){
		float vol = 0f;
		vol = value / volumeSlider.maxValue;
		AudioListener.volume = vol;
	}


}
