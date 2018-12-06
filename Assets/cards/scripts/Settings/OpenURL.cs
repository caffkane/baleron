using UnityEngine;
using System.Collections;

public class OpenURL : MonoBehaviour {

	public string ios_url = "URL";
	public string wp8_url = "URL";
	public string and_url = "URL";
	public string else_url = "URL";

	// Use this for initialization
	public void loadurl() {
#if UNITY_IOS
		Application.OpenURL(ios_url);
		return;
#endif

#if UNITY_ANDROID
		Application.OpenURL(and_url);
		return;
#endif

#if (UNITY_WP8||UNITY_WP8_1||UNITY_WSA)
		Application.OpenURL(wp8_url);
		return;
#endif

#if !UNITY_IOS && !UNITY_ANDROID && !(UNITY_WP8||UNITY_WP8_1||UNITY_WSA)
		Application.OpenURL(else_url);
#endif

	}
}