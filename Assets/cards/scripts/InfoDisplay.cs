using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoDisplay : MonoBehaviour {

	public static InfoDisplay instance;

	void Start(){
		instance = this;
		valuesToDisplay = new List<valueSprite> ();
		valuesToDisplay.Clear ();

		StartCoroutine (cyclicIconCollector ());
	}

	IEnumerator cyclicIconCollector(){

		while (true) {

			//wait for at least one icon to display
			while (valuesToDisplay.Count <= 0) {
				yield return null;
			}

			yield return new WaitForSeconds (collectionTime);

			startAnimationIfNotEmpty ();

			yield return new WaitForSeconds (displayTime);

			clearDisplay ();
		}
	}

	public Animator anim;


	private List<valueSprite> valuesToDisplay;

	public Sprite emptySprite;

	[Tooltip("'collectionTime' defines the duration for collecting new icons before showing them.")]
	[Range(0.1f, 1f)] public float collectionTime = 0.5f;
	[Tooltip("'displayTime' defines the duration for showing icons before start collecting again.")]
	[Range(0.1f, 5f)] public float displayTime = 2f;
	public Color positiveColor = Color.green;
	public Color negativeColor = Color.red;


	public void startAnimationIfNotEmpty(){

		if (valuesToDisplay.Count > 0) {
			if (anim != null) {
				anim.SetTrigger ("show");
			}
		}
	}

	public void addDisplay(Sprite s, float number){
		valueSprite v = new valueSprite();
		v.sprite = s;
		v.number = number;
		valuesToDisplay.Add (v);
		fillDisplayElements ();
	}

	void fillDisplayElements(){
		int nrToDisplay = valuesToDisplay.Count;
		int nrOfSlots = changeDisplaySlots.Length;

		for(int i = 0; i < changeDisplaySlots.Length; i++) {
			if (i < nrToDisplay) {
				changeDisplaySlots [i].img.sprite = valuesToDisplay [i].sprite;
				changeDisplaySlots [i].txt.text = valuesToDisplay [i].number.ToString();

				if (valuesToDisplay [i].number >= 0f) {
					changeDisplaySlots [i].img.color = positiveColor;
					changeDisplaySlots [i].txt.color = positiveColor;
				} else {
					changeDisplaySlots [i].img.color = negativeColor;
					changeDisplaySlots [i].txt.color = negativeColor;
				}
					
			} else {
				//clean the not used slots
				changeDisplaySlots [i].img.sprite = emptySprite;
				changeDisplaySlots [i].txt.text = "";

			}
		}
	}

	public void clearDisplay(){
		valuesToDisplay.Clear ();
		fillDisplayElements ();
	}

	[System.Serializable]
	public class valueSprite{
		public Sprite sprite;
		public float number;
	}

	[System.Serializable]
	public class displaySlot{
		public Image img;
		public Text txt;
	}

	public displaySlot[] changeDisplaySlots;


}
