using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AchievementsScript : TranslatableContent {
	
	public static AchievementsScript instance;

	void Awake(){
		instance = this;
	}

	/*
	 * Specification of all the possible achivements.
	 */
	public enum achievementTyp{
		marry,
		rule_years
	}

	[Tooltip("If an achivement was triggered it can be shown with an animator. Link this animator here.")]
	public Animator achievementAnimator;
	[Tooltip("If an achivement was triggered it can be shown with an animator. Specify the trigger for the animator here.")]
	public string triggerOnAchievement = "show";
	[Tooltip("If an achivement was triggered it can update information about the achivement. Specify the placeholder title text display here.")]
	public Text anim_titleText;
	[Tooltip("If an achivement was triggered it can update information about the achivement. Specify the placeholder description text display here.")]
	public Text anim_descriptionText;
	[Tooltip("If an achivement was triggered it can update information about the achivement. Specify the placeholder image display here.")]
	public Image anim_achievementImage;

	[Tooltip("The achievement progress ('3/20') can be displayed. Specify the placeholder text display here.")]
	public Text achievementProgressText;

	[System.Serializable]
	public class achievement{
		public achievementTyp typ;
		public int achievementCnt;
	}

	[System.Serializable]
	public class achievementConfig{
		[Tooltip("Specification the type of achievement here. For expanding the list modify the structure 'achievementTyp' within this script.")]
		public achievementTyp typ;
		[Tooltip("The number, how often the achievement was met. The animator for displaying the achievement is only triggered the first time.")]
		[ReadOnlyInspector]public int achievementCnt;

		public achievementStage[] achievementStages;

		public void save(){
			SecurePlayerPrefs.SetInt("achievement_" + typ.ToString(), achievementCnt);
		}

		public void load(){
			achievementCnt = SecurePlayerPrefs.GetInt("achievement_" + typ.ToString());
		}
	}

	[System.Serializable]
	public class achievementStage{
		[Tooltip("This Stage of the achievement ist triggered if this counter is reached")]
		public int achievementTarget;
		[Tooltip("If the achievement was met, a gameobject can be activated. E.g. display the achievement in a gallery.")]
		public GameObject achievementGameobject;
		[Tooltip("The title text for the achievement. If you want to use translations, this text can be used as term.")]
		public string title;
		[Tooltip("The description text for the achievement. If you want to use translations, this text can be used as term.")]
		public string description;
		[Tooltip("The sprite for the achievement. If you want to use translations, this text can be used as term.")]
		public Sprite sprite;
	}

	public achievementConfig[] achievements;

	void load(){
		foreach (achievementConfig ac in achievements) {
			ac.load ();
		}

		countAndShowAchieventProgesss ();
	}

	public void countAndShowAchieventProgesss(){
		int achievementsOverall = 0;
		int achievementsDone = 0;
		foreach (achievementConfig ac in achievements) {
			achievementsOverall++;
			if (ac.achievementCnt > 0 ) {
				achievementsDone++;
			}
		}

		if (achievementProgressText != null) {
			achievementProgressText.text = achievementsDone.ToString () + "/" + achievementsOverall.ToString ();
		}
	}

	public void activateGameObjects(){
		foreach (achievementConfig ac in achievements) {
			int stageReached = getAchievementStageReached (ac);

			for(int i = 0; i < ac.achievementStages.Length ; i++) {
				if (i == stageReached) {
					//activate, if stage is reached and it is not active yet
					if (ac.achievementStages [i].achievementGameobject.activeSelf == false) {
						ac.achievementStages [i].achievementGameobject.SetActive (true);
					}
				} else {
					//deactivate, if stage is other than reached one and it is active yet
					if (ac.achievementStages [i].achievementGameobject.activeSelf == true) {
						ac.achievementStages [i].achievementGameobject.SetActive (false);
					}
				}
			}
		}
	}
		
	void Start () {
		load ();
		activateGameObjects ();	//activate the gameobjects e.g. in gallery for the already scored achievements
		TranslationManager.instance.registerTranslateableContentScript (this);
	}

	public void addAchievement(achievementTyp typ){
		load ();
		bool playAnimation = false;

		foreach (achievementConfig ac in achievements) {
			if (ac.typ == typ) {
				ac.achievementCnt++;
				ac.save ();

				foreach (achievementStage aStage in ac.achievementStages) {
					if (ac.achievementCnt == aStage.achievementTarget) {
						playAnimation = true;
					}
				}
				if (playAnimation == true) {
					showAchievementAnimation (ac);
				}

				activateGameObjects ();
				countAndShowAchieventProgesss ();
			}
		}
	}

	public void setAchievementCounter(achievementTyp typ, int newAchievementCounter){
		load ();
		bool playAnimation = false;

		foreach (achievementConfig ac in achievements) {
			if (ac.typ == typ) {

				//only accept new counter values, if bigger than previous reached ones.
				if (newAchievementCounter > ac.achievementCnt) {
					ac.achievementCnt = newAchievementCounter;

					foreach (achievementStage aStage in ac.achievementStages) {
						if (ac.achievementCnt == aStage.achievementTarget) {
							playAnimation = true;
						}
					}
					if (playAnimation == true) {
						showAchievementAnimation (ac);
					}

					activateGameObjects ();
					countAndShowAchieventProgesss ();
				}

				ac.save ();
			}
		}
	}

	int getAchievementStageReached(achievementConfig ac){
		int maxStage = -1;

		for(int i = 0; i < ac.achievementStages.Length; i++){
			if (ac.achievementCnt >= ac.achievementStages[i].achievementTarget) {
				maxStage = i;
			}
		}
		return maxStage;
	}

	void showAchievementAnimation(achievementConfig ac){

		int stage = getAchievementStageReached (ac);

		if (achievementAnimator != null) {
			if (anim_descriptionText != null) {
				anim_descriptionText.text = TranslationManager.translateIfAvail(ac.achievementStages[stage].description);
			}
			if (anim_titleText != null) {
				anim_titleText.text = TranslationManager.translateIfAvail(ac.achievementStages[stage].title);
			}
			if (anim_achievementImage != null) {
				anim_achievementImage.sprite = ac.achievementStages[stage].sprite;
			}
			achievementAnimator.SetTrigger (triggerOnAchievement);
		}
	}

	/*
	 * Return all possible translatable terms
	 */
	public override List<string> getTranslatableTerms ()
	{
		List<string> terms = new List<string> ();
		terms.Clear ();
		EventScript es;

		foreach (achievementConfig ac in achievements) {
			foreach (achievementStage aStage in ac.achievementStages) {
				terms.Add (aStage.description);
				terms.Add (aStage.title);
			}
		}

		return terms;
	}
}
