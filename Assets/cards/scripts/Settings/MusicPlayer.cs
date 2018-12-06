using UnityEngine;
using System.Collections;


public class MusicPlayer : MonoBehaviour {

	[Tooltip("List of possible audio clips for playing in the game.")]
	public AudioClip[] _audioClips;
	[Tooltip("Audiosource for playing the audio clips.")]
	public AudioSource mainAudio;
	[Tooltip("Select one of the audi clips randomly at activation of the gameobject.")]
	public bool playRandomAtStart = false;
	[Tooltip("Loop the selected audio clip (or play all the clips in the list).")]
	public bool loopSong = false;


	private int nrOfSources;
	private AudioClip  actualSource;
	private int actualIndex = 0;

	public bool testNext = false;
	private float songDuration = 0f;

	[HideInInspector][Range(0.0f, 5f)] [SerializeField] private float fadeDuration = 0f; 

	void Start () {
		nrOfSources = _audioClips.Length;


		if (playRandomAtStart == true) {
			int rnd = Random.Range (0, nrOfSources);
			actualIndex = rnd;
		} else {
			actualIndex = 0;
		}
	
		actualSource = _audioClips [actualIndex];
		mainAudio.clip = _audioClips [actualIndex];

		StartCoroutine (multimediaTimer ());

		songDuration = mainAudio.clip.length;	//memorize the song-duration
		mainAudio.Play ();

		if (loopSong == true) {
			mainAudio.loop = true;
		}
	}

	IEnumerator multimediaTimer(){
	
		float now = Time.realtimeSinceStartup;
		float last = now;


		while (true) {
			now = Time.realtimeSinceStartup;

			songDuration-= (now-last);

			if(loopSong == false){
			if(songDuration<fadeDuration){
				nextListedSong();


				if(songDuration <=fadeDuration){
						Debug.LogWarning("An audioclip  is shorter than the fading time, this can cause strange sound behaviour.");
				}
			}
			}else{
				//no pre-time for fading out, if the song replays. This can't be controlled with the volume-Slider.
				if(songDuration<0f){
					//nextListedSong();
										
					//if(songDuration <=fadeDuration){
					//	Debug.LogWarning("An audioclip  is shorter than the fading time, this can cause strange sound behavior.");
					//}
				}
			}
		
			last = now;

			yield return new WaitForSeconds(0.1f); //this is not accurate. 
		}

	}

	public void nextSong(){

		mainAudio.loop = false;

		actualIndex++;
		if (actualIndex >= nrOfSources) {
			actualIndex = 0;
		}
		actualSource = _audioClips [actualIndex];
		mainAudio.clip = actualSource;
		mainAudio.Play ();
		mainAudio.loop = true;
		songDuration = mainAudio.clip.length;
		loopSong = false;		//the user breaks the loop, we reload this value on next ending song.

	}

	private void replaySong(){

		actualSource = _audioClips [actualIndex];
		mainAudio.clip = actualSource;
		mainAudio.Play ();
		songDuration = mainAudio.clip.length;
	}

	private void nextListedSong(){
		if (loopSong == true) {
			replaySong();
		} else {
			nextSong();
		}
	}
	
	void Update () {
		if (testNext == true) {
			nextSong();
			testNext = false;
		}
	}

	void OnDisable(){
		mainAudio.Stop ();
		StopAllCoroutines ();

	}

	void OnEnable(){
		if (actualSource != null) {
			songDuration = mainAudio.clip.length;	//memorize the song-duration
			mainAudio.Play ();
			StartCoroutine (multimediaTimer ());
		}
	}
}
