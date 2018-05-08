using UnityEngine;
using System.Collections;

public class TransitionManager : MonoBehaviour {

	public Texture2D texture;
	public float fadeSpeed = 0.8f;

	private int drawDepth = -1000;
	private float alpha = 1f;
	private int fadeDir = -1;

	public static TransitionManager instance = null;

	void Start() {
		if (instance == null)
			instance = this;
		else if (instance != this)
			this.enabled = false;
	}

	void OnGUI () {
		alpha += fadeDir * fadeSpeed * Time.deltaTime;

		if (alpha > 1) {
			fadeDir = 0;
		}

		alpha = Mathf.Clamp01 (alpha);

		if (alpha > 0) {
			GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);
			GUI.depth = drawDepth;
			GUI.DrawTexture (Camera.main.pixelRect, texture);
		}

	}

	public float BeginFade (int direction) {
		fadeDir = direction;
		return 1 / fadeSpeed;
	}

	public void SetAlpha(float a) {
		alpha = a;
	}

	// accepts 3 parameterless functions - one is called before the transition, one is called in the middle, and one is called at the end
	public IEnumerator TransitionScene(System.Action start, System.Action middle, System.Action end) {
		if(start != null)
			start ();
		yield return null;

		BeginFade (1);

		float time = BeginFade (1);
		yield return new WaitForSeconds (time);

		SetAlpha (1);

		if(middle != null)
			middle ();
		yield return null;

		time = BeginFade (-1);
		yield return new WaitForSeconds (time);

		SetAlpha (0);

		if(end != null)
			end ();
		yield return null;
	}
}
