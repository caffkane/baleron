using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class LoadScene : MonoBehaviour {

	public string LevelToLoad = "";
	public float timeToLoad = 0.1f;

	private void compute_levelload(){
				SceneManager.LoadScene (LevelToLoad);
			}

	public void levelload(){
		StartCoroutine (delayedLoad ());
	}

	IEnumerator delayedLoad(){
		yield return new WaitForSeconds (timeToLoad);
		compute_levelload ();
	}
					
}

	

