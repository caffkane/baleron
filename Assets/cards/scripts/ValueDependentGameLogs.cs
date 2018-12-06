using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueDependentGameLogs : TranslatableContent {

	public static ValueDependentGameLogs instance;

	void Awake(){
		instance = this;
	}
	void Start(){
		TranslationManager.instance.registerTranslateableContentScript (this);
	}

	/*
	 * By call of 'executeValueLogging' all the conditions are computed and
	 * accordingly gamelogs are added. To avoid duplicate gamelogs, this should only
	 * be called e. g. at the end of an game. By the option 'lockOutput' all further game logging is 
	 * disabled.
	 */
	public void executeValueLogging(bool lockOutput = false){
		foreach (valueLog vl in logConditions) {
			if (valueManager.instance.getConditionMet (vl.condition) == true) {
				GameLogger.instance.addGameLog (vl.logConditionMet);
			}
		}
		GameLogger.instance.lockOutput (lockOutput);
		GameLogger.instance.saveGameLogs ();
	}

	[System.Serializable]
	public class valueLog{

		public EventScript.condition condition;
		public string logConditionMet = "";

	}

	public valueLog[] logConditions;

	/*
	 * Return all possible translatable terms
	 */
	public override List<string> getTranslatableTerms ()
	{
		List<string> terms = new List<string> ();
		terms.Clear ();

		foreach (valueLog vl in logConditions) {
			terms.Add (vl.logConditionMet);
		}

		return terms;
	}
}
