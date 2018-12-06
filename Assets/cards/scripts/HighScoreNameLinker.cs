using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

/*
 * The script 'HighScoreNameLinker' takes a high score (from multible possible sources)
 * and combines it with the player identity where it origins from. 
 * The highscores and the identity are shown some frames after the start of the game.
 */

public class HighScoreNameLinker : MonoBehaviour {

    [System.Serializable]
    public class highScoreNamePair {

        public string countryName;
        public int highScore;
        //public bool valid = false;
    }


    //[ReadOnlyInspector]public highScoreNamePair hsnPair;

        //encapsulate inside class to enable json serialisation
    [System.Serializable]
    public class C_cap{
        [Tooltip("Loaded/actual highscore pair.")]
        public List<highScoreNamePair> list = new List<highScoreNamePair>();
}
public C_cap hsnPairs;


    public enum hsnSelection
	{
		valueScript,
		scoreCounter
	}
	[Tooltip("Select the source of the score.")]
	public hsnSelection highScoreSource;

    public enum hsnSorting {
        highest,
        lowest,
        lastOnTop
    }

    [Tooltip("Select the how the score should be sorted.")]
    public hsnSorting highScoreSort;

    [Tooltip("Save key is auto generated from the linked scripts.")]
	[HideInInspector]public string key = "saveKey";
	public scoreCounter sc;
	ValueScript vs;
	public valueDefinitions.values valueType;

    [System.Serializable]
    public class C_highscoreFields
    {
        [Tooltip("Select text field to show the player identity with the score.")]
        public Text countryNameText;
        [Tooltip("Select text field to show the score.")]
        public Text highScoreText;
    }
    public C_highscoreFields[] highScoreFields;

	void Awake(){
	}

	void Start(){
		clearUI ();
		StartCoroutine (delayed ());
	}

	IEnumerator delayed(){
		yield return null;
		yield return null;
		yield return null;
		generateSaveKey ();
		load ();
		displayHighScorePair ();
	}

	void getVSscript(){
		vs = valueManager.instance.getFirstFittingValue (valueType);
	}

    void generateSaveKey()
    {
        switch (highScoreSource)
        {
            case hsnSelection.valueScript:
                getVSscript();
                key = vs.valueType.ToString();
                break;
            case hsnSelection.scoreCounter:
                key = sc.key;
                break;
            default:
                key = "unknown";
                break;
        }

        switch (highScoreSort)
        {
            case hsnSorting.highest:
                key += "_HS_pair_highest";
                break;
            case hsnSorting.lowest:
                key += "_HS_pair_lowest";
                break;
            case hsnSorting.lastOnTop:
                key += "_HS_pair_last";
                break;
            default:
                key += "_HS_pair_undefined";
                break;

        }
    }

	public void displayHighScorePair(){

        for (int i = 0; i < highScoreFields.Length; i++)
        {

            if (i < hsnPairs.list.Count)
            {
                //if an entry for place 1, 2 ... exists, write the highscore value and the country name text

                if (highScoreFields[i].countryNameText != null)
                {
                    highScoreFields[i].countryNameText.text = hsnPairs.list[i].countryName;
                }
                if (highScoreFields[i].highScoreText != null)
                {
                    highScoreFields[i].highScoreText.text = hsnPairs.list[i].highScore.ToString();
                }
            }
            else {
                //if it doesn't exists:


                if (highScoreFields[i].countryNameText != null)
                {
                    highScoreFields[i].countryNameText.text = "";
                }
                if (highScoreFields[i].highScoreText != null)
                {
                    highScoreFields[i].highScoreText.text = "";
                }
            }
        }
	}

	public void clearUI(){
        foreach (C_highscoreFields hsf in highScoreFields)
        {
            if (hsf.countryNameText != null)
            {
                hsf.countryNameText.text = "";
            }
            if (hsf.highScoreText != null)
            {
                hsf.highScoreText.text = "";
            }
        }
	}

	void save(){
		PlayerPrefs.SetString(key, JsonUtility.ToJson(hsnPairs));
	}

	void load(){
		if (PlayerPrefs.HasKey (key)) {
			string json = PlayerPrefs.GetString (key);
			JsonUtility.FromJsonOverwrite (json, hsnPairs);
		}

        //else {
			//no save exists, generate one.
		//	hsnPair.countryName = "none";
		//	if (highScoreSource == hsnSelection.valueScriptMinimal) {
		//		hsnPair.highScore = 9999;
		//	} else {
		//		hsnPair.highScore = 0;
		//	}
		//	hsnPair.valid = false;
		//	save ();
		//}
	}


	/*
	 * Generate the actual pair of the high score and the player identity.
	 * This function is called by 'HighScoreNameLinkerGroup' by the method 'generateLinks()' to
	 * generate all at once.
	 */
	public void generateHighScoreNameLink(){
		int score = 0;
        //bool newHighScore = false;

        //Debug.LogWarning("generateHighScoreNameLink");

		generateSaveKey ();
		load ();

        switch (highScoreSource)
        {
            case hsnSelection.valueScript:
                getVSscript();
                score = Mathf.RoundToInt(vs.value);
                break;
            case hsnSelection.scoreCounter:
                score = sc.getScore();
                break;
            default:
                score = 0;
                break;
        }
		
        //if the player got gameover, add a new highscore entry
        highScoreNamePair hsnp = new highScoreNamePair();
        hsnp.highScore = score ;
        //hsnp.valid = true;
        hsnp.countryName = CountryNameGenerator.instance.getCountryNameText(); ;

        //sort the highscore, depending on selection
        switch (highScoreSort)
        {
            case hsnSorting.lowest:
                hsnPairs.list.Add(hsnp);
                hsnPairs.list.Sort(SortByLowerScore);
                break;
            case hsnSorting.highest:
                hsnPairs.list.Add(hsnp);
                hsnPairs.list.Sort(SortByHigherScore);
                break;
            case hsnSorting.lastOnTop:
                //add on top, don't sort
                hsnPairs.list.Insert(0, hsnp);
                break;
            default:
                hsnPairs.list.Insert(0, hsnp);
                break;
        }

        int maxNumberOfEntries = 5;
        if (2 * highScoreFields.Length > maxNumberOfEntries) {
            maxNumberOfEntries = 2 * highScoreFields.Length;
        }

        if (hsnPairs.list.Count > maxNumberOfEntries) {
            //keep the highscore list size small
            hsnPairs.list = hsnPairs.list.GetRange(0, maxNumberOfEntries - 1);
        }

        save();

}

    static int SortByHigherScore(highScoreNamePair p1, highScoreNamePair p2)
    {
        return p2.highScore.CompareTo(p1.highScore);
    }
    static int SortByLowerScore(highScoreNamePair p1, highScoreNamePair p2)
    {
        return p1.highScore.CompareTo(p2.highScore);
    }
}
