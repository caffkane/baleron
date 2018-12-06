using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//With ValueChangePreview the images and text for an upcoming value change can defined.
//There can be multible sets of images, the can be changed at runtime.
//The image and text for the preview itself is linked from each ValueScript.

public class ValueChangePreview : MonoBehaviour {
	public static ValueChangePreview instance;

	void Awake(){
		if (instance == null) {
			instance = this;
		} else {
			Debug.LogError ("Multiple 'ValueChangePreview' configurations, this is error prone.");
		}
	}

	public int selectedSpriteset = 0;

	[System.Serializable]
	public class valueChangeSpriteSet{
		[Tooltip("Define the preview sprite if the value rises.")]
		public Sprite valueRises;
		[Tooltip("Define the preview sprite if the value falls.")]
		public Sprite valueFalls;
		[Tooltip("Define the sprite if the change of the value is not clear. This can happen if the outcome will be randomized in some way.")]
		public Sprite valueUnclear;
		[Tooltip("Define the scale of the sprite depending on the value change. Adjust the time axis to the value change. E. g. your value can change from 0 to 100 then define the size for a time from 0 to 100.")]
		public AnimationCurve spriteSize;
        [Tooltip("Show the as text it this set is used.")]
        public bool showTextValue = true;
		[Tooltip("If text is displayed a formatter can be defined here. E. g. '0' for showing whole numbers.")]
		public string textFormatter = "0";
	}

	public valueChangeSpriteSet[] spriteSets;
	[Tooltip("Define a sprite if nothing changes. If nothing should be displayed supply an transparent sprite.")]
	public Sprite noChanges;


	/// <summary>
	/// Sets the sprite-set index for showing different sets.
	/// </summary>
	/// <param name="spriteSetIndex">Sprite set index.</param>
	public void setSpriteSet(int spriteSetIndex){
		if (spriteSetIndex >= 0 && spriteSetIndex < spriteSets.Length) {
			selectedSpriteset = spriteSetIndex;
		} else {
			if (spriteSets.Length >= 1) {
				Debug.LogError ("The selection of sprite set " + spriteSetIndex.ToString () + " is not possible. The index of the sprite sets on 'ValueChangePreview' are defined from 0 to " + (spriteSets.Length-1).ToString() +".");
			} else {
				Debug.LogError ("The selection of sprite set " + spriteSetIndex.ToString () + " is not possible. There are no sprite sets defined on 'ValueChangePreview'.");
			}
		}
	}

	//for the value script: return scale of the image dependant of value changes
	public float getPreviewSize(bool changes, float value, bool randomIndependant){
		if (changes == false) {
			return 0f;
		}

		if (randomIndependant == true) {
			return spriteSets [selectedSpriteset].spriteSize.Evaluate (Mathf.Abs ((float)value));
		} else {
			return 1f;
		}
	}

	//for the value script: return sprite dependant of value changes
	public Sprite getPreviewSprite(bool changes,float value, bool randomIndependant){
		if (changes == false) {
			return noChanges;
		}

		if (randomIndependant == true) {
			if (value >= 0f) {
				return spriteSets [selectedSpriteset].valueRises;
			} else {
				return spriteSets [selectedSpriteset].valueFalls;
			}
		} else {
			return spriteSets[selectedSpriteset].valueUnclear;
		}
	}

	//for the value script: return text dependant of value changes
	public string getPreviewText(bool changes,float value, bool randomIndependant){
        if (spriteSets[selectedSpriteset].showTextValue == true)
        {
            if (changes == false)
            {
                return "";
            }

            if (randomIndependant == true)
            {
                return value.ToString(spriteSets[selectedSpriteset].textFormatter);
            }
            else
            {
                return "?";
            }
        }
        else {
            return "";
        }
	}

}
