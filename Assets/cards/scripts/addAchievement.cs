using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class AddGameLog is a helper to allow 
 * adding of an achievement by an event.
 * E.g. this script can be added to an card and by 
 * calling 'add_Achievement' from the 'OnCardDespawn' - event it
 * will increase the achievemet counter for the predefined 
 * achievement type.
 */

public class addAchievement : MonoBehaviour {

	public AchievementsScript.achievementTyp achievement;

	public void add_Achievement(){
		if (AchievementsScript.instance != null) {
			AchievementsScript.instance.addAchievement (achievement);
		}
	}

	public void set_AchievementCounter(int newAchievementCounter){
		if (AchievementsScript.instance != null) {
			AchievementsScript.instance.setAchievementCounter (achievement, newAchievementCounter);
		}
	}
}
