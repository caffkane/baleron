#define KINGS_GAME_ASSET_ADS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

//if Unity Ads are configured correctly, the code between '#if UNITY_ADS' and '#endif' 
//is executed. If it is grayed out, the ads are not available.

#if UNITY_ADS
using UnityEngine.Advertisements; // only compile Ads code on supported platforms
#endif

public class PlayUnityAd : MonoBehaviour {

	const string placement = "rewardedVideo";

	[System.Serializable] public class mEvent : UnityEvent {}
	public mEvent onAdAvailable;
	public mEvent onAdNotAvailable;
	public mEvent onAdSuccess;
	public mEvent onAdFail;

	#if KINGS_GAME_ASSET_ADS
	public setValue valueIfAvailable;
	public setValue valueIfNotAvailable;
	#endif

	[Tooltip("'watchAdButton' is optional. If an advertisement is available, the button will be set to interactable. To show ad with button, call 'showRewardedAd()'.")]
	public Button watchAdButton;

	[Tooltip("Is the ad rewarded? If yes, the callbacks on success/fail will be called and the user can't skip the Ad. Else the default will be played.")]
	public bool rewardedAd = true;

	void Start () {
		if (watchAdButton != null) {
			watchAdButton.interactable = false;
		}
		StartCoroutine (testForAdvertisement());

#if !UNITY_STANDALONE
#if !UNITY_ADS
        Debug.LogWarning("Unity Ads is not enabled.");
#endif
#endif

	}

	//Assume advertising is not available at the beginning.
	bool advertisementAvailable = false;
	bool advertisementTested = false;

	//cyclic test, if ad is available and enable/disable the 'watchAdButton' and generate 'onAdAvailable' event
	IEnumerator testForAdvertisement(){

		yield return null;
		onAdNotAvailable.Invoke ();

		yield return new WaitForSecondsRealtime (0.5f);

		while (true) {
#if UNITY_ADS
			if (( Advertisement.IsReady (placement) && rewardedAd == true) || (Advertisement.IsReady() && rewardedAd == false )) {

				if (watchAdButton != null) {
					watchAdButton.interactable = true;
				}

				if (advertisementAvailable == false) {
					advertisementAvailable = true;
					onAdAvailable.Invoke ();
#if KINGS_GAME_ASSET_ADS
					valueIfAvailable.setValues();
#endif
				}
				//Debug.Log ("Ad is ready");
			} else {
				if (watchAdButton != null) {
					watchAdButton.interactable = false;
				}
				//generate 'ad not available' event on first start or if it was available bevore
				if (advertisementAvailable == true || advertisementTested == false) {
					advertisementAvailable = false;
					advertisementTested = true;
					onAdNotAvailable.Invoke ();
#if KINGS_GAME_ASSET_ADS
					valueIfNotAvailable.setValues();
#endif
				}
			//	Debug.Log ("Ad is NOT ready");
			}

#endif
				

			yield return new WaitForSecondsRealtime (1f);
		} 
	}

	/*
	 * Call 'showAd' to start play an ad.
	 * The type of ad is depending on configuration 'rewardedAd'
	 */
	public void showAd(){
		if (rewardedAd == true) {
			showRewardedAd ();
		} else {
			showDefaultAd ();
		}
	}

	/*
	 * Call 'showRewardedAd' to start play an ad.
	 * If it fails (stopped or not available), the event 'onAdFail' is invoked.
	 * If it was successful, the event 'onAdSuccess' is invoked.
	 */
	private void showRewardedAd(){

#if UNITY_ADS
		if (!Advertisement.IsReady(placement))
		{
			HandleShowResult(ShowResult.Failed);
			return;
		}else{
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show(placement, options);
		}
#endif
	}

	/*
	 * Call 'showDefaultAd' to start play an ad.
	 * If it fails (stopped or not available), the event 'onAdFail' is invoked.
	 * If it was successful, the event 'onAdSuccess' is invoked.
	 */
	private void showDefaultAd(){
#if UNITY_ADS
		if (!Advertisement.IsReady())
		{
			HandleShowResult(ShowResult.Failed);
			return;
		}else{
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show(options);
		}
#endif
	}

#if UNITY_ADS
	private void HandleShowResult(ShowResult result)
	{
		switch (result) {
		case ShowResult.Finished:
			onAdSuccess.Invoke ();
			break;
		case ShowResult.Skipped:
			//Debug.Log ("The ad was skipped before reaching the end.");
			onAdFail.Invoke ();
			break;
		case ShowResult.Failed:
			//Debug.LogError ("The ad failed to be shown.");
			onAdFail.Invoke ();
			break;
		}
	}
#endif

}
