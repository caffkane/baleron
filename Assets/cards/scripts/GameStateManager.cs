using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour {

	[System.Serializable] public class mEvent : UnityEvent {}

	//count the swipes to create the event for the first swipe in game for switching the menu
	[HideInInspector] public int swipeCounter = 0;

	public static GameStateManager instance;

	public enum Gamestate
	{
		idle,
		gameActive,
		gameOver
	}

	[Tooltip("Actual state of the game.")]
	[ReadOnlyInspector] public Gamestate gamestate = Gamestate.idle;

	void loadGameState(){
		gamestate  = (Gamestate)PlayerPrefs.GetInt ("GameState") ;
	}

	void saveGameState(){
		PlayerPrefs.SetInt("GameState",(int)gamestate);
	}


	void Awake(){
		instance = this;
		loadGameState ();
	}

	// Use this for initialization
	void Start () {
		StartCoroutine (OneFrameDelayStartup ());
	}

	IEnumerator OneFrameDelayStartup(){

		//because of Awake-instance linking and registering within startup,
		//we need at least one frame delay to start the game.

		yield return null;
		yield return null;
		GameStartup ();
	}


	void GameStartup(){
		//if we start with a gameover from the last game the game goes to idle.
		if (gamestate == Gamestate.gameOver) {
			gamestate = Gamestate.idle;
		}

		//if we are idle we trigger the start of a new game
		if (gamestate == Gamestate.idle) {
			StartGame ();
		}
	}
		
	public void executeGameover(){
		gamestate = Gamestate.gameOver;

        //Debug.LogWarning("executeGameover");

		if (gamesPlayedCounter != null) {
			gamesPlayedCounter.increase (1);	//log the number of played games
		}
			
		valueManager.instance.saveAllMinMaxValues ();			//save min and max values for all values for the statistics tab
		HighScoreNameLinkerGroup.instance.generateLinks ();		
		CardStack.instance.resetCardStack ();					//reset the card stack

		saveGameState ();
		string currentSceneName = SceneManager.GetActiveScene ().name;
		SceneManager.LoadScene (currentSceneName);						//reload the scene for a clean startup of the game
	}

	public mEvent OnNewGame;
	public mEvent OnFirstSwipe;

	public void swipe(){
		swipeCounter++;

		if (swipeCounter == 1) {
			OnFirstSwipe.Invoke ();
		}
	}


	void StartGame(){
		swipeCounter = 0;
		if (gamestate == Gamestate.idle) {

			//do game start preparations
			OnNewGame.Invoke();

			CountryNameGenerator.instance.actualizeTexts (true);
			GenderGenerator.instance.actualizeUI ();
			GameLogger.instance.clearGameLog ();			//delete the last game log for the new game

			gamestate = Gamestate.gameActive;
			saveGameState ();
		}
	}

	void OnDestroy(){
		saveGameState ();
	}

	public scoreCounter gamesPlayedCounter;
}
