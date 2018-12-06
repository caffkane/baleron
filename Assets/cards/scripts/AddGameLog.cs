using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class AddGameLog is a helper to allow 
 * adding of an gamelog by an event.
 * E.g. this script can be added to an card and by 
 * calling 'add_Achivement' from the 'OnCardDespawn' - event 
 * it will add an entry to the game log.
 */

public class AddGameLog : MonoBehaviour {

	public void addGameLogText(string txt){
		GameLogger.instance.addGameLog (txt);
	}
}
