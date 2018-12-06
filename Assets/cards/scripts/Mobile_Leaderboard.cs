#define USE_SECURE_PLAYERPREFS
//#define USE_MOBILE_LEADERBOARD

using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

#if USE_MOBILE_LEADERBOARD
//Defined by compilation configuration
#if		UNITY_ANDROID 	
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using GooglePlayGames.BasicApi;
#endif

#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif

#endif

/// <summary>
/// Leaderboard and Achievements for Google Play and iOS Gamecenter
/// </summary>

public class Mobile_Leaderboard : MonoBehaviour {


	[Tooltip("Provide a (big) textfield to see the status debug output of the script when testing on a device.")]
	public Text StatusText;
	[Tooltip("Provide a textfield to show achievements to the user.")]
	public Text achievementText;


	public bool AutoConnectToGoogleAtStartup = true;
	public bool showAchievementsUIAfterProzessing = false;

	[Tooltip("Define the scores to transmit. Needs some keys and strings from online configuration.")]
	public scoreSet[] scoresToTransmit;
	[Tooltip("Define possible achievements. Needs some keys and strings from online configuration.")]
	public achievement[] achievements;

	private bool __newAchivements = false;
	private bool __stateAuth = false;
	private bool __stateTransmittingScores = false;
	private bool __stateCallingLeaderboard = false;

	private bool __leaderboardRequested = false;
	private bool __achievementRequested = false;
	private bool __achievementUIRequested = false;
	private bool __LeaderboardUIRequested = false;


	private bool __stateAuth_a = false;
	private bool __stateTransmittingAchievements_a = false;
	private bool __stateCallingAchievementUI_a = false;

	public string autoConnectKey = "ML_Autoconnect"; //prefs load key for the autoconnect state, if it is UI configurable

	string debugText = "";
	string oldTxt;
	void mDebug(string txt){
		if (string.Equals(oldTxt,txt)==false) {
			debugText += txt + "\n";
			if (StatusText != null) {
				StatusText.text = debugText;
			}
			oldTxt = txt;
		}
	}

	void setAchievmentText(string txt){
		if (achievementText != null) {
			achievementText.text = txt;
		}
	}

	void OnEnable () {
		#if USE_MOBILE_LEADERBOARD
		#if UNITY_IOS
		StartCoroutine(RegisterForGameCenter());
#endif
		#endif
	}


	IEnumerator sozialActivations(){

		//yield return new WaitForSeconds(0.1f);
		#if USE_MOBILE_LEADERBOARD
		#if		UNITY_ANDROID 	
		
		GooglePlayGames.PlayGamesPlatform.Activate();
		
		#endif
		
		
		//test_authenticate_only ();
		
		mDebug ("Start");
		getAutoconnectSetting ();
		
		if (AutoConnectToGoogleAtStartup == true) {
			__stateAuth = true;
			authenticate();
		}
		#endif
		yield return new WaitForSeconds(0.1f);
	}

	void Awake(){
		StartCoroutine (sozialActivations ());
	}
	/// <summary>
	/// Can be called by an UI event (Button): Refresh the settings
	/// </summary>
	public void UI_refreshSettings(){
		StartCoroutine (refreshSettingsDelayed ());
	}
	IEnumerator refreshSettingsDelayed(){
		yield return new WaitForSecondsRealtime (0.1f);
		getAutoconnectSetting ();
	}

	void getAutoconnectSetting(){
		if (PlayerPrefs.HasKey (autoConnectKey)) {
			int connect = PlayerPrefs.GetInt (autoConnectKey);
			if (connect == 0) {
				AutoConnectToGoogleAtStartup = false;
			} else {
				AutoConnectToGoogleAtStartup = true;
			}
		} else {
			//if (AutoConnectToGoogleAtStartup == true) {
			//	PlayerPrefs.SetInt (autoConnectKey, 1);
			//} else {
			//	PlayerPrefs.SetInt (autoConnectKey, 0);
			//}
		}
	}
	
	void Start () {
		getAutoconnectSetting ();
	}

	//Call this, if you only want to authenticate() (connect to google play)
	public void UI_call_authenticate(){
		__stateAuth = true;
		authenticate ();
	}


	//Call this, when opening the Leaderboard by a button.
	public void UI_call_transmitscoreAndLeaderboard(){
		mDebug ("Beging transmit and Score");
		authenticate ();

		__stateAuth = true;
		__leaderboardRequested = true;
		__LeaderboardUIRequested = true;
	}

	//Call this, if only the scores have to be Transmited
	public void std_call_transmitScore(){
		if (AutoConnectToGoogleAtStartup == true) {
			mDebug ("Authenticate");
			authenticate ();
			mDebug ("Request for leaderboard score transmission.");
			__stateAuth = true;
			__leaderboardRequested = true;
			__LeaderboardUIRequested = false;
		}
	}

	//Call this, when opening the Achievements by a button.
	public void UI_call_computeAchievements(){
		mDebug ("Beging connect and achievements");
		__stateAuth = true;
		__stateAuth_a = true;				//connecting and
		__achievementRequested = true;		//updating achievements

		authenticate ();
		std_call_computeAchievements ();
		__achievementUIRequested = true;	//and showing the ui
	}

	//Call this, when just computing the Achivements. It opens the 
	//AchivementUI, only when there are new achievements and the disigner wants to.
	public void std_call_computeAchievements(){

		// 1. Testing. Are there new Achievements, which are not reported?
		mDebug("Test for Achievements");
		 __newAchivements = test_for_new_achievements ();

		// 2. Yes?
		if (__newAchivements == true) {
			authenticate ();
			__stateAuth_a = true;					//connecting and
			__achievementRequested = true;		//updating achievements
			__achievementUIRequested = showAchievementsUIAfterProzessing;	//and showing the ui, if the designer wants so
			mDebug("achievements started.");
			//Debug.Log("achievements stuff startet");
		} else {
			//do nothing. No connection needed, because we got no new achievements
			mDebug("nothing to unlock.");
		}

	}

	public void std_call_computeAchievements_ifConfigured(){
		getAutoconnectSetting ();
		if (AutoConnectToGoogleAtStartup == true) {
			std_call_computeAchievements ();
		}
	}

	bool compute_achievements(){
		#if USE_MOBILE_LEADERBOARD
		int achievementState = 0;
		int keyValue = 0;
		int i = 0;
		if (achievements != null) {															//are there Achievements?
			foreach (achievement a in achievements) {										//for each of them

				keyValue = readScoreFromPlayerprefs(a.scoreKey);	

				if (keyValue >= a.MinScoreValue) {											//and test, if is reached
					achievementState = PlayerPrefs.GetInt (a.AchievementName + "_state");	//and already reported.
					a.__reached = true;														//When it is reached
					if (achievementState == 0 || a.__transmitted == true) {					//and it is not already reported,
						if(a.achievement_message!=null){
						setAchievmentText (a.achievement_message.text);						//tell the congratulations to the user.
						}
						if (a.__transmitRequested == false) {								//Then test, if there was a request for transmit

							if (a.__transmitted == false) {									//and if it was transmitted.
								#if UNITY_IOS
								KTGameCenter.SharedCenter().SubmitAchievement(100,a.AchievementID,true);
								#endif
								#if		UNITY_ANDROID 
								Social.ReportProgress (a.AchievementID, 100.0f, (bool success) => {	//ok, then we can report it, but:
									// handle success or failure
									if (success) {
										PlayerPrefs.SetInt (a.AchievementName + "_state", 1);
										//achievement_reported = true;

										a.__transmitted = true;
									} else {
										//Debug.Log ("no");
									}

									a.__transmitted = true;
								});
								#endif
								a.__transmitted = true;
								a.__transmitRequested = true;								//memorize, we requested the transmit.
							}
							return false;
						}
					}
				}	
				i++;
			}
		}
		return true;
		#else
		return false;
		#endif
	}

	bool test_for_new_achievements(){
		int achievementState = 0;
		int keyValue = 0;
		//bool achievement_reported = false;
		int i = 0;
		if (achievements != null) {															//are there Achievements?
			foreach (achievement a in achievements) {										//for each of them
				keyValue = readScoreFromPlayerprefs(a.scoreKey);							//get the actual Value

				if (keyValue >= a.MinScoreValue) {											//and test, if is reached
					achievementState = PlayerPrefs.GetInt (a.AchievementName + "_state");	//and already reported.
					a.__reached = true;														//When it is reached
					if (achievementState == 0) {											//and it is not already reported,
						if(a.achievement_message!=null){
						setAchievmentText (a.achievement_message.text);						//tell the congratulations to the user.
						}
						return true;
					}
				}	
				i++;
			}
		}
		return false;
	}


	void Update () {

		if(__leaderboardRequested == true){
			cyclic_for_leaderboard();
		}

		if (__achievementRequested == true) {
			cyclic_for_achievement();
		}
	}

	public void test_authenticate_only(){
	#if USE_MOBILE_LEADERBOARD
		#if		UNITY_ANDROID 
			Social.localUser.Authenticate (success => {
				if (success) {
					//Debug.Log ("Authentication successful");
					string userInfo = "Username: " + Social.localUser.userName + 
						"\nUser ID: " + Social.localUser.id + 
							"\nIsUnderage: " + Social.localUser.underage;
					//Debug.Log (userInfo);
				}
				else{
					//Debug.Log ("Authentication failed");
			}
			});
		#endif
	#if UNITY_IOS
	KTGameCenter.SharedCenter().Authenticate();
	#endif
	#endif
		}
		

	void cyclic_for_achievement (){

		if (__stateAuth_a == true) {
			if (Social.localUser.authenticated) {
				__stateAuth_a = false;
				__stateTransmittingAchievements_a = true;
			}
			mDebug ("Waiting for authentication");
		}
		
		
		if (__stateTransmittingAchievements_a == true) {
			mDebug("Sending Achievements.");

			bool result = compute_achievements ();
				
			if(result == true){	
				__stateTransmittingAchievements_a = false;
				__stateCallingAchievementUI_a = true;
			}
		}
		
		if (__stateCallingAchievementUI_a == true) {
			
			mDebug("Sending done, opening ui.");	//cleaning Info-String
			
			// show leaderboard UI
			if(__achievementUIRequested == true){
				Social.ShowAchievementsUI();
				__achievementUIRequested = false;
			}
			
			__stateCallingAchievementUI_a = false;
			//__stateIdle_a = true;
		}

	}

	void cyclic_for_leaderboard(){
		if (__stateAuth == true) {
			if (Social.localUser.authenticated) {
				__stateAuth = false;
				__stateTransmittingScores = true;
			}
			mDebug ("Waiting for authentication");
		}
		
		
		if (__stateTransmittingScores == true) {
			mDebug("Sending Scores.");
			if(scoresToTransmit!=null){
				foreach(scoreSet ss in scoresToTransmit){
					if(ss.__transmitRequested == false){
						if(ss.__transmitted == false){
							ss.__transmitRequested = true;
							reportScore(ss);
						}
						return;
					}
				}
				
				__stateTransmittingScores = false;
				__stateCallingLeaderboard = true;
			}else{
				__stateTransmittingScores = false;
				__stateCallingLeaderboard = true;
			}
		}
		
		if (__stateCallingLeaderboard == true) {
			
			mDebug("");	//cleaning Info-String
			
			// show leaderboard UI
			if(__LeaderboardUIRequested == true){
				Social.ShowLeaderboardUI();
				__LeaderboardUIRequested = false;
			}
			
			__stateCallingLeaderboard = false;
			//__stateIdle = true;
		}
	}

	void reportScores(){
	
	}

	//read score as int and float.
	//return the higher value as int
	int readScoreFromPlayerprefs(string key){
		#if USE_SECURE_PLAYERPREFS
		float f_value = SecurePlayerPrefs.GetFloat (key);
		int i_value = SecurePlayerPrefs.GetInt (key);
		#else
		float f_value = PlayerPrefs.GetFloat (key);
		int i_value = PlayerPrefs.GetInt (key);
		#endif
		int i_f_value = Mathf.RoundToInt (f_value);
		int result = 0;
		if (i_f_value > i_value) {
			result = i_f_value;
		} else {
			result = i_value;
		}
		return result;
	}

	void reportScore (scoreSet m_score){

		//get the different memorized score values
		m_score.__scoreActual = readScoreFromPlayerprefs( m_score.scoreKey);
		m_score.__scoreMax = readScoreFromPlayerprefs(m_score.maxScoreKey);
		m_score.__score_maxTransmitted = readScoreFromPlayerprefs(m_score.scoreKey + "_maxTransmit");

		//select the score to transmit
		if (m_score.__scoreMax > m_score.__score_maxTransmitted) {
			m_score.__scoreToTransmit = m_score.__scoreMax;
		} else {
			m_score.__scoreToTransmit = m_score.__scoreActual;
		}

		mDebug ("score: " + m_score.__scoreActual.ToString());
		mDebug ("score max: " + m_score.__scoreMax.ToString());
		mDebug ("score max trans.: " + m_score.__score_maxTransmitted.ToString());
		mDebug ("score to transmit: " + m_score.__score_maxTransmitted.ToString ());

		// post score 12345 to leaderboard ID "Cfji293fjsie_QA")
		Social.ReportScore(m_score.__scoreToTransmit, m_score.leaderBoardID, (bool success) => {
			// handle success or failure
			if(success){
				m_score.__transmitOk = true;

				if(m_score.__scoreToTransmit>m_score.__score_maxTransmitted){
					#if USE_SECURE_PLAYERPREFS
					SecurePlayerPrefs.SetFloat (m_score.scoreKey + "_maxTransmit",(float)(m_score.__scoreToTransmit));
					#else
					PlayerPrefs.SetFloat (m_score.scoreKey + "_maxTransmit",(float)(m_score.__scoreToTransmit));
					#endif
				}

			}else{
				m_score.__transmitFail = true;
			}
			m_score.__transmitted = true;
		});
	}


	void authenticate(){
	#if USE_MOBILE_LEADERBOARD
		#if		UNITY_ANDROID 
		// Authenticate
		//mWaitingForAuth = true;
		mDebug("Authenticating...");
		if(Social.localUser.authenticated == false){
		Social.localUser.Authenticate((bool success) => {
			//mWaitingForAuth = false;
			if (success) {
				mDebug("Welcome " + Social.localUser.userName);
				std_call_computeAchievements();
			} else {
				mDebug("Authentication failed.");
			}
		});
		}
#endif
		#if UNITY_IOS
		KTGameCenter.SharedCenter().Authenticate();
#endif
	#endif

	}

	public void UI_Sign_out(){
	#if USE_MOBILE_LEADERBOARD
		mDebug("Signing out.");

		#if		UNITY_ANDROID 	
		((GooglePlayGames.PlayGamesPlatform) Social.Active).SignOut();
		#endif

		//sign out only for Android?
	#endif
	}
#if USE_MOBILE_LEADERBOARD
	#if UNITY_IOS
	void OnDisable () {
		KTGameCenter.SharedCenter().GCUserAuthenticated -= GCAuthentication;
		KTGameCenter.SharedCenter().GCScoreSubmitted -= ScoreSubmitted;
		KTGameCenter.SharedCenter().GCAchievementSubmitted -=
			AchievementSubmitted;
		KTGameCenter.SharedCenter().GCAchievementsReset -= AchivementsReset;
	}
	IEnumerator RegisterForGameCenter () {
		yield return new WaitForSeconds(0.5f);
		KTGameCenter.SharedCenter().GCUserAuthenticated += GCAuthentication;
		KTGameCenter.SharedCenter().GCScoreSubmitted += ScoreSubmitted;
		KTGameCenter.SharedCenter().GCAchievementSubmitted +=
			AchievementSubmitted;
		KTGameCenter.SharedCenter().GCAchievementsReset += AchivementsReset;
	}
	void GCAuthentication (string status) {
		mDebug ("delegate call back status= "+status);

		//UI_call_computeAchievements();
		std_call_computeAchievements();
	}
	
		void ScoreSubmitted (string leaderboardId,string error) {
			print ("score submitted with id "+leaderboardId +" and error= "+error);
		}
		void AchievementSubmitted (string achId,string error) {
			print ("achievement submitted with id "+achId +" and error= "+error);
		}
		void AchivementsReset (string error) {
			print ("Achievement reset with error= "+error);
		}
#endif
#endif
		[System.Serializable]
	public class scoreSet
	{
		[Tooltip("From online configuration, name of the leaderboard.")]
		public string leaderBoardName;
		[Tooltip("From online configuration, ID of the leaderboard.")]
		public string leaderBoardID;
		[Tooltip("Score is loaded from preferences. Provide the according key here.")]
		public string scoreKey;
		[Tooltip("Maximum score is loaded from preferences. Provide the according key here.")]
		public string maxScoreKey;
		public bool __transmitOk{ get; set; }
		public bool __transmitFail{ get; set; }
		public bool __transmitted{ get; set; }
		public bool __transmitRequested { get; set; }
		public int __scoreActual{ get; set; }
		public int __scoreMax{ get; set; }
		public int __score_maxTransmitted{ get; set; }
		public int __scoreToTransmit{ get; set; }
	}

	[System.Serializable]
	public class achievement
	{
		[Tooltip("From online configuration, name of the achievement.")]
		public string AchievementName;
		[Tooltip("From online configuration, ID of the achievement.")]
		public string AchievementID;
		public Text achievement_message;
		[Tooltip("If an score exceeds an minimal value, the achievement is triggered. Score is loaded from preferences. Provide the according key here.")]
		public string scoreKey;
		[Tooltip("Define the minimum value to trigger the achievement.")]
		public int MinScoreValue;
		public bool __reached { get; set; }
		public bool __transmitOk{ get; set; }
		public bool __transmitFail{ get; set; }
		public bool __transmitted{ get; set; }
		public bool __transmitRequested { get; set; }
	}
}
