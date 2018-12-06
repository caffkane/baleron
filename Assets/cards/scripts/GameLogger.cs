

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * Logger to give a summary of the played game at the end of the game.
 */

public class GameLogger : TranslatableContent {



	public static GameLogger instance;



	/*
	 * Returns the game log as text.
	 */
	public string getGameLog(){
		return buildResultText ();	
	}

	/*
	 * Shows the gamelog at the defined text box.
	 */
	[HideInInspector]public Text gameLogText;
	public void showGameLogUI(){
		if (gameLogText != null) {
			gameLogText.text = buildResultText ();
		}
	}

	[System.Serializable]
	public class strList
	{
		public bool locked = false;
		public List <string> gameLogs;
	}

	[Tooltip("Gamelog of the actual game.")]
	[ReadOnlyInspector]public strList logs;



	[Tooltip("'textBreakEvery' generates a linebreak ever x logs to format the output string in a more readable text.")]
	public int textBreakEvery = 1;

	string buildResultText(){
		string result = "";
		int lineCnt = 0;

		foreach (string s in logs.gameLogs) {
			result = result + TranslationManager.translateIfAvail(s) + " ";

			lineCnt++;
			if (lineCnt >= textBreakEvery) {
				result = result + "\n\n";
				lineCnt = 0;
			}
		}
		return result;
	}


	void Awake(){
		instance = this;
	}

	void Start(){
		logs.gameLogs = new List<string> ();
		logs.gameLogs.Clear();
		loadGameLogs ();
		TranslationManager.instance.registerTranslateableContentScript (this);
	}

	/*
	 * Force to load the gamelogs. This is autmatically done at start of the script.
	 */
	public bool loadGameLogs(){
		string json = PlayerPrefs.GetString ("gameLog");
		if (string.IsNullOrEmpty (json)) {
			return false;
		} else {
			JsonUtility.FromJsonOverwrite (json, logs);

			return true;
		}
	}

	/*
	 * Locking the logs prevents the script from adding new logs.
	 * This is needed, because the loading of a running game can cause cards to add already logged logs.
	 */
	public void lockOutput(bool doLock){
		logs.locked = doLock;
	}

	public void saveGameLogs(){
		string json = JsonUtility.ToJson (logs);
		PlayerPrefs.SetString ("gameLog", json);
	}

	/*
	 * Add a new log-entry for this game, if the logger is not locked.
	 */
	public void addGameLog(string log){
		if (!string.IsNullOrEmpty (log) && logs.locked == false) {

			string txt = log;
			logs.gameLogs.Add (txt);
			saveGameLogs ();

		}
	}

	/*
	 * By calling 'clearGameLog' the gamelog is cleared and the lock is removed.
	 */
	public void clearGameLog(){
		logs.gameLogs.Clear ();
		logs.locked = false;
		saveGameLogs ();
	}

	/*
	 * Return all possible translatable terms
	 */
	public override List<string> getTranslatableTerms ()
	{
		List<string> terms = new List<string> ();
		terms.Clear ();

		Debug.LogWarning ("Strings of 'GameLogger' are not directly listed and therefore can't be completely added to translation term list. ");

		return terms;
	}
}
