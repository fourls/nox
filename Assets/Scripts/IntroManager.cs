using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroManager : MonoBehaviour {

	public Text madeByText;
	public Text elliotText;
	public string madeBy;
	public string elliot;

	// Use this for initialization
	void Start () {
		StartCoroutine (PlayIntro ());
	}

	IEnumerator PlayIntro() {
		madeByText.text = "";
		elliotText.text = "";

		yield return new WaitForSeconds (0.2f);

		for (int i = 0; i < madeBy.Length; i++) {
			madeByText.text += madeBy [i];
			yield return new WaitForSeconds (0.1f);
		}

		yield return new WaitForSeconds (0.1f);

		for (int i = 0; i < elliot.Length; i++) {
			elliotText.text += elliot [i];
			yield return new WaitForSeconds (0.1f);
		}

		yield return new WaitForSeconds (2f);

		TransitionManager.instance.StartCoroutine(TransitionManager.instance.TransitionScene (null, MiddleTransition, null));
	}

	void MiddleTransition() {
		UnityEngine.SceneManagement.SceneManager.LoadScene ("LevelSelect");
		Destroy (gameObject);
	}
}
