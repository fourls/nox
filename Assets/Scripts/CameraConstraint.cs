using UnityEngine;
using System.Collections;

public class CameraConstraint : MonoBehaviour {

	public GameObject tooSmallPrefab;

	private bool fullscreen = false;
	private Rect camRect;
	private GameObject tooSmallObject = null;

	void Start () {
		camRect = new Rect((Screen.width/2)-(1280/2),(Screen.height/2)-(800/2),1280,800);
		GetComponent<Camera> ().pixelRect = camRect;

		Reevaluate ();
	}

	void Update () {
		if (Screen.fullScreen != fullscreen || GetComponent<Camera> ().pixelRect != camRect) {
			Reevaluate ();
		}
	}

	void Reevaluate() {
		fullscreen = Screen.fullScreen;
		if (fullscreen) {
			Screen.SetResolution (Screen.resolutions [Screen.resolutions.Length - 1].width,Screen.resolutions [Screen.resolutions.Length - 1].height,fullscreen);
		}
		if (Screen.height < 800 || Screen.width < 1280) {
			if (tooSmallObject == null) {
				tooSmallObject = (GameObject)Instantiate (tooSmallPrefab);
			}
		} else if(tooSmallObject != null) {
			Destroy (tooSmallObject);
			tooSmallObject = null;
		}
		camRect = new Rect((Screen.width/2)-(1280/2),(Screen.height/2)-(800/2),1280,800);
		GetComponent<Camera> ().pixelRect = camRect;
	}
}
